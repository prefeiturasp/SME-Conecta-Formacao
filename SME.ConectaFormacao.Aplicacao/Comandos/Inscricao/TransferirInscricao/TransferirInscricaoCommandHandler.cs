using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TransferirInscricaoCommandHandler : IRequestHandler<TransferirInscricaoCommand, RetornoInscricaoDTO>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;
        private readonly IRepositorioProposta _repositorioProposta;

        public TransferirInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao, IRepositorioCargoFuncao repositorioCargoFuncao, IMediator mediator, IRepositorioProposta repositorioProposta, IRepositorioUsuario repositorioUsuario)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<RetornoInscricaoDTO> Handle(TransferirInscricaoCommand request, CancellationToken cancellationToken)
        {
            ValidarTransferencia(request.InscricaoTransferenciaDTO);

            var cursistaBanco = new Usuario();

            var retorno = new RetornoInscricaoDTO
            {
                Cursistas = new List<Dtos.Usuario.CursistaDTO>()
            };

            foreach (var cursista in request.InscricaoTransferenciaDTO.Cursistas)
            {
                try
                {
                    var inscricao = await _repositorioInscricao.ObterPorId(cursista.IdInscricao);

                    cursistaBanco = await _repositorioUsuario.ObterPorLogin(cursista.Rf)
                        ?? throw new NegocioException(
                            string.Format(MensagemNegocio.USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS, cursista),
                            HttpStatusCode.NotFound);

                    ValidarInscricao(inscricao);

                    var inscricaoNovaManual = new InscricaoManualDTO
                    {
                        PropostaTurmaId = request.InscricaoTransferenciaDTO.IdTurmaDestino,
                        Cpf = cursistaBanco.Cpf,
                        RegistroFuncional = cursistaBanco.Login
                    };

                    var cargos = await _mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(inscricaoNovaManual.RegistroFuncional));

                    inscricaoNovaManual.CargoCodigo = cargos?.FirstOrDefault()?.CdCargoBase?.ToString();
                    inscricaoNovaManual.FuncaoCodigo = cargos?.FirstOrDefault()?.CdFuncaoAtividade?.ToString();
                    inscricaoNovaManual.CargoUeCodigo = cargos?.FirstOrDefault()?.CdUeCargoBase?.ToString();

                    if (inscricaoNovaManual.CargoCodigo == null)
                    {
                        var cargosOutros = await _mediator.Send(new ObterCargoFuncaoOutrosQuery());
                        inscricaoNovaManual.CargoCodigo = cargosOutros?.Id.ToString();
                    }

                    var propostaTurmaDestino = await _repositorioProposta.ObterTurmaDaPropostaComDresPorId(
                        request.InscricaoTransferenciaDTO.IdTurmaDestino);

                    if (propostaTurmaDestino != null)
                    {
                        ValidarDreTransferencia(propostaTurmaDestino, inscricao.CargoDreCodigo);

                        if (cargos != null && cargos.Any(c => c.CdCargoBase != null))
                        {
                            ValidarCargoTransferencia(inscricao.CargoCodigo, inscricaoNovaManual.CargoCodigo);
                        }

                        var proposta = (await _repositorioProposta.ObterPropostasResumidasPorId(new[] { propostaTurmaDestino.PropostaId })).SingleOrDefault();

                        if (proposta == null)
                            throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

                        await _repositorioProposta.AtualizarIntegrarNoSGA(proposta.Id, PropostaIntegrarNoSGA.NAO.ToBool());

                        await _mediator.Send(new SalvarInscricaoManualCommand(inscricaoNovaManual, true));

                        await _repositorioInscricao.AtualizarSituacao(inscricao.Id, SituacaoInscricao.Transferida);
                        await _repositorioInscricao.LiberarInscricaoVaga(inscricao);
                    }
                }
                catch (Exception ex)
                {
                    retorno.Cursistas.Add(new Dtos.Usuario.CursistaDTO
                    {
                        NomeCursista = cursistaBanco.Nome,
                        Rf = !string.IsNullOrEmpty(cursista.Rf) ? Convert.ToInt64(cursista.Rf) : 0,
                        Mensagem = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Erro ao transferir inscrição."
                    });
                }
            }

            if (retorno.Cursistas.Any())
            {
                retorno.Status = (int)HttpStatusCode.BadRequest;
                retorno.Mensagem = "Falha ao realizar transferência(s).";
            }
            else
            {
                retorno.Status = (int)HttpStatusCode.OK;
                retorno.Mensagem = "Todas as inscrições foram transferidas com sucesso.";
            }

            return retorno;
        }
        private static void ValidarInscricao(Inscricao inscricao)
        {
            if (inscricao is null)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            if (inscricao.Situacao == SituacaoInscricao.Transferida || inscricao.Situacao == SituacaoInscricao.Cancelada)
                throw new NegocioException(MensagemNegocio.INSCRICOES_CANCELADAS, HttpStatusCode.BadRequest);
        }

        private static void ValidarTransferencia(InscricaoTransferenciaDTO dto)
        {
            if (dto.IdTurmaDestino == dto.IdTurmaOrigem)
                throw new NegocioException(MensagemNegocio.INSCRICAO_MESMA_TURMA_ORIGEM_DESTINO, HttpStatusCode.BadRequest);

            if (dto.Cursistas == null || !dto.Cursistas.Any())
                throw new NegocioException(MensagemNegocio.CURSISTA_INFORMAR, HttpStatusCode.BadRequest);
        }

        private static void ValidarDreTransferencia(PropostaTurma propostaTurmaDestino, string dreCodigoOrigem)
        {
            if (propostaTurmaDestino == null)
                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            if (propostaTurmaDestino.Dres == null || !propostaTurmaDestino.Dres.Any())
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL, HttpStatusCode.NotFound);

            var dreCodigoDestino = propostaTurmaDestino.Dres.FirstOrDefault()?.DreId;

            if (dreCodigoDestino == null || dreCodigoDestino == 0)
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL, HttpStatusCode.NotFound);

            if (string.IsNullOrWhiteSpace(dreCodigoOrigem))
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL, HttpStatusCode.NotFound);
        }

        private static void ValidarCargoTransferencia(string cargoOrigem, string cargoDestino)
        {
            if (cargoOrigem == null || cargoOrigem == "")
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL, HttpStatusCode.NotFound);

            if (cargoDestino == null || cargoDestino == "")
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL, HttpStatusCode.NotFound);

            if (cargoOrigem != cargoDestino)
                throw new NegocioException(MensagemNegocio.INSCRICAO_TRANSFERENCIA_CARGOS_DIFERENTES, HttpStatusCode.BadRequest);
        }
    }
}
