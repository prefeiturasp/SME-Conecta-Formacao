using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TransferirInscricaoCommandHandler : IRequestHandler<TransferirInscricaoCommand, RetornoDTO>
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

        public async Task<RetornoDTO> Handle(TransferirInscricaoCommand request, CancellationToken cancellationToken)
        {
            var cursistasErro = new List<string>();
            var inscricoesSucesso = new List<Inscricao>();

            ValidarTransferencia(request.InscricaoTransferenciaDTO);

            foreach (var cursista in request.InscricaoTransferenciaDTO.Cursistas)
            {
                var inscricao = await _repositorioInscricao.ObterPorId(cursista.IdInscricao);

                ValidarInscricao(inscricao);

                var cursistaBanco = await _repositorioUsuario.ObterPorLogin(cursista.Rf)
                    ?? throw new NegocioException(
                        string.Format(MensagemNegocio.USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS, cursista),
                        HttpStatusCode.NotFound);

                try
                {
                    var inscricaoNovaManual = new InscricaoManualDTO
                    {
                        PropostaTurmaId = request.InscricaoTransferenciaDTO.IdTurmaDestino,
                        Cpf = cursistaBanco.Cpf,
                        RegistroFuncional = cursistaBanco.Login
                    };

                    var cargos = await _mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(inscricaoNovaManual.RegistroFuncional));
                    var cargosOutros = await _mediator.Send(new ObterCargoFuncaoOutrosQuery());

                    inscricaoNovaManual.CargoCodigo = cargos?.FirstOrDefault()?.CdCargoBase?.ToString() ?? cargosOutros?.Id.ToString();
                    inscricaoNovaManual.FuncaoCodigo = cargos?.FirstOrDefault()?.CdFuncaoAtividade?.ToString();
                    inscricaoNovaManual.CargoUeCodigo = cargos?.FirstOrDefault()?.CdUeCargoBase?.ToString();

                    var propostaTurmaDestino = await _repositorioProposta.ObterTurmaDaPropostaComDresPorId(
                        request.InscricaoTransferenciaDTO.IdTurmaDestino);

                    if (propostaTurmaDestino != null)
                    {
                        ValidarDreTransferencia(propostaTurmaDestino, inscricao.CargoDreCodigo);

                        if (!string.IsNullOrEmpty(inscricaoNovaManual.CargoCodigo) &&
                            !string.IsNullOrEmpty(inscricao.CargoCodigo) &&
                            inscricaoNovaManual.CargoCodigo != cargosOutros?.Id.ToString())
                        {
                            ValidarCargoTransferencia(inscricao.CargoCodigo, inscricaoNovaManual.CargoCodigo);
                        }

                        var proposta = (await _repositorioProposta.ObterPropostasResumidasPorId(new[] { propostaTurmaDestino.PropostaId }))
                                       .SingleOrDefault();

                        if (proposta == null)
                            throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

                        await _repositorioProposta.AtualizarIntegrarNoSGA(proposta.Id, PropostaIntegrarNoSGA.NAO.ToBool());

                        await _mediator.Send(new SalvarInscricaoManualCommand(inscricaoNovaManual));

                        inscricoesSucesso.Add(inscricao);

                        await _repositorioInscricao.AtualizarSituacao(inscricao.Id, SituacaoInscricao.Transferida);
                        await _repositorioInscricao.LiberarInscricaoVaga(inscricao);
                    }
                }
                catch (Exception ex)
                {
                    cursistasErro.Add($"{cursistaBanco.Nome} ({cursistaBanco.Login}) - {ex.Message}");
                }
            }

            if (cursistasErro.Any())
            {
                var mensagem = "Processamento concluído com algumas falhas.\n\n" +
                               "Erros:\n" + string.Join("\n", cursistasErro) +
                               $"\n\n{inscricoesSucesso.Count} inscrições foram transferidas com sucesso.";
                return RetornoDTO.RetornarErro(mensagem);
            }

            return RetornoDTO.RetornarSucesso("Todas as inscrições foram transferidas com sucesso.");
        }

        private void ValidarInscricao(Inscricao inscricao)
        {
            if (inscricao is null)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            if (inscricao.Situacao == SituacaoInscricao.Transferida || inscricao.Situacao == SituacaoInscricao.Cancelada)
                throw new NegocioException(MensagemNegocio.INSCRICOES_CANCELADAS, HttpStatusCode.BadRequest);
        }

        private void ValidarTransferencia(InscricaoTransferenciaDTO dto)
        {
            if (dto.IdTurmaDestino == dto.IdTurmaOrigem)
                throw new NegocioException(MensagemNegocio.INSCRICAO_MESMA_TURMA_ORIGEM_DESTINO, HttpStatusCode.BadRequest);

            if (dto.Cursistas == null || !dto.Cursistas.Any())
                throw new NegocioException(MensagemNegocio.CURSISTA_INFORMAR, HttpStatusCode.BadRequest);
        }

        private void ValidarDreTransferencia(PropostaTurma propostaTurmaDestino, string dreCodigoOrigem)
        {
            if (propostaTurmaDestino == null)
                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            if (propostaTurmaDestino.Dres == null || !propostaTurmaDestino.Dres.Any())
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL);

            var dreCodigoDestino = propostaTurmaDestino.Dres
                .FirstOrDefault()?
                .DreId;

            if (dreCodigoDestino == null || dreCodigoDestino == 0)
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL);

            if (string.IsNullOrWhiteSpace(dreCodigoOrigem))
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL);
        }

        private void ValidarCargoTransferencia(string cargoOrigem, string cargoDestino)
        {
            if (cargoOrigem == null || cargoOrigem == "")
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL);

            if (cargoDestino == null || cargoDestino == "")
                throw new NegocioException(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL);

            if (cargoOrigem != cargoDestino)
                throw new NegocioException(MensagemNegocio.INSCRICAO_TRANSFERENCIA_CARGOS_DIFERENTES);
        }

    }
}
