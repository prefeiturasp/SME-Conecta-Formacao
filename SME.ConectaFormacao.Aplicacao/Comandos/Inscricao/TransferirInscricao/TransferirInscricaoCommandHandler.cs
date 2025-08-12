using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TransferirInscricaoCommandHandler : IRequestHandler<TransferirInscricaoCommand, RetornoDTO>
    {
        private readonly ITransacao _transacao;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;
        private readonly IRepositorioProposta _repositorioProposta;

        public TransferirInscricaoCommandHandler(ITransacao transacao, IRepositorioInscricao repositorioInscricao, IRepositorioCargoFuncao repositorioCargoFuncao, IMediator mediator, IRepositorioProposta repositorioProposta, IRepositorioUsuario repositorioUsuario)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<RetornoDTO> Handle(TransferirInscricaoCommand request, CancellationToken cancellationToken)
        {
            var cursistasErro = new List<string>();

            var inscricao = await _repositorioInscricao.ObterPorId(request.IdInscricao);

            ValidarInscricao(inscricao);
            ValidarTransferencia(request.InscricaoTransferenciaDTO);

            var usuarioLogado = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia(), cancellationToken)
                ?? throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var inscricaoNovaManual = new InscricaoManualDTO();

            foreach (var cursista in request.InscricaoTransferenciaDTO.Cursistas)
            {
                var cursistaBanco = await _repositorioUsuario.ObterPorLogin(cursista.ToString())
                    ?? throw new NegocioException(
                        string.Format(MensagemNegocio.USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS, cursista),
                        HttpStatusCode.NotFound);

                try
                {
                    inscricaoNovaManual.PropostaTurmaId = request.InscricaoTransferenciaDTO.IdTurmaDestino;
                    inscricaoNovaManual.Cpf = cursistaBanco.Cpf;
                    inscricaoNovaManual.RegistroFuncional = cursistaBanco.Login;

                    var query = new ObterCargosFuncoesDresFuncionarioServicoEolQuery(inscricaoNovaManual.RegistroFuncional);
                    var cargos = await _mediator.Send(query);

                    if (cargos != null && cargos.Any())
                    {
                        inscricaoNovaManual.CargoCodigo = cargos.First().CdCargoBase.ToString();
                        inscricaoNovaManual.FuncaoCodigo = cargos.First().CdFuncaoAtividade.ToString();
                        inscricaoNovaManual.CargoUeCodigo = cargos.First().CdUeCargoBase.ToString();

                        var propostaTurmaDestino = await _repositorioProposta.ObterTurmaDaPropostaComDresPorId(request.InscricaoTransferenciaDTO.IdTurmaDestino);

                        if (propostaTurmaDestino != null)
                        {
                            ValidarDreTransferencia(propostaTurmaDestino, inscricao.CargoDreCodigo);

                            if (!string.IsNullOrEmpty(inscricaoNovaManual.CargoCodigo) && !string.IsNullOrEmpty(inscricao.CargoCodigo))
                                ValidarCargoTransferencia(inscricao.CargoCodigo, inscricaoNovaManual.CargoCodigo);

                            await _mediator.Send(new SalvarInscricaoManualCommand(inscricaoNovaManual));
                        }
                    }
                }
                catch (Exception ex)
                {
                    cursistasErro.Add($"{cursistaBanco.Nome} ({cursistaBanco.Login}) - {ex.Message}");
                }
            }

            if (!cursistasErro.Any())
            {
                inscricao.Situacao = SituacaoInscricao.Transferida;
                await _repositorioInscricao.Atualizar(inscricao);
                await _repositorioInscricao.LiberarInscricaoVaga(inscricao);

                return RetornoDTO.RetornarSucesso("Todas as inscrições foram transferidas com sucesso.");
            }
            else
            {
                var mensagem = "Processamento concluído com falhas.\n\n" +
                    "\n\nErros:\n" + string.Join("\n", cursistasErro) +
                    "\n\nA inscrição original não foi alterada pois houve falhas.";

                return RetornoDTO.RetornarSucesso(mensagem);
            }
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
