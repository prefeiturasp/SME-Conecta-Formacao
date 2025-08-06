using AutoMapper;
using MediatR;
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
            var inscricao = await _repositorioInscricao.ObterPorId(request.IdInscricao)
                ?? throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            ValidarInscricao(inscricao);
            ValidarTransferencia(request.InscricaoTransferenciaDTO);

            var usuarioLogado = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia(), cancellationToken)
                ?? throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var transacao = _transacao.Iniciar();

            try
            {
                inscricao.Situacao = SituacaoInscricao.Transferida;
                await _repositorioInscricao.Atualizar(inscricao);
                await _mediator.Send(new EnviarEmailCancelarInscricaoCommand(request.IdInscricao, "Inscrição transferida"), cancellationToken);
                await _repositorioInscricao.LiberarInscricaoVaga(inscricao);

                var inscricaoNova = new Inscricao();
                inscricaoNova.Situacao = SituacaoInscricao.Confirmada;
                inscricaoNova.UsuarioId = usuarioLogado.Id;
                inscricaoNova.Origem = OrigemInscricao.Transferencia;
                inscricaoNova.PropostaTurmaId = request.InscricaoTransferenciaDTO.IdTurmaDestino;

                var propostaTurmaDestino = await _repositorioProposta.ObterTurmaDaPropostaComDresPorId(request.InscricaoTransferenciaDTO.IdTurmaDestino);

                ValidarDreTransferencia(propostaTurmaDestino, inscricao.CargoDreCodigo);

                await _repositorioInscricao.Inserir(inscricaoNova);

                foreach (var cursista in request.InscricaoTransferenciaDTO.Cursistas)
                {
                    var cursistaBanco = await _repositorioUsuario.ObterPorLogin(cursista.ToString())
                        ?? throw new NegocioException(string.Format(MensagemNegocio.USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS, cursista), HttpStatusCode.NotFound);

                    var dto = new InscricaoAutomaticaDTO
                    {
                        PropostaId = propostaTurmaDestino.PropostaId,
                        PropostaTurmaId = propostaTurmaDestino.Id,

                        CargoId = inscricao.CargoId,
                        CargoCodigo = inscricao.CargoCodigo,
                        CargoDreCodigo = inscricao.CargoDreCodigo,
                        CargoUeCodigo = inscricao.CargoUeCodigo,

                        FuncaoId = inscricao.FuncaoId,
                        FuncaoCodigo = inscricao.FuncaoCodigo,
                        FuncaoDreCodigo = inscricao.FuncaoDreCodigo,
                        FuncaoUeCodigo = inscricao.FuncaoUeCodigo,

                        UsuarioId = cursistaBanco.Id,
                        UsuarioRf = cursistaBanco.Login,
                        UsuarioNome = cursistaBanco.Nome,
                        UsuarioCpf = cursistaBanco.Cpf,

                        TipoVinculo = inscricao.TipoVinculo
                    };

                    await _mediator.Send(new PublicarNaFilaRabbitCommand(
                        RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista,
                        dto
                    ));
                }


                transacao.Commit();
                return RetornoDTO.RetornarSucesso("Inscrição transferida com sucesso", inscricaoNova.Id);
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
        private void ValidarInscricao(Inscricao inscricao)
        {
            if (inscricao is null)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            if (inscricao.Situacao == SituacaoInscricao.Cancelada)
                throw new NegocioException(MensagemNegocio.INSCRICOES_CANCELADAS, HttpStatusCode.BadRequest);
        }

        private void ValidarTransferencia(InscricaoTransferenciaDTO dto)
        {
            if (dto.IdTurmaDestino == dto.IdTurmaOrigem)
                throw new NegocioException(MensagemNegocio.INSCRICAO_MESMA_TURMA_ORIGEM_DESTINO, HttpStatusCode.BadRequest);

            if (dto.Cursistas == null || !dto.Cursistas.Any())
                throw new NegocioException("Cursista(s) não informado(s).", HttpStatusCode.BadRequest);
        }

        private void ValidarDreTransferencia(PropostaTurma propostaTurmaDestino, string dreCodigoOrigem)
        {
            if (propostaTurmaDestino == null)
                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            if (propostaTurmaDestino.Dres == null || !propostaTurmaDestino.Dres.Any())
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL);

            var dreCodigoDestino = propostaTurmaDestino.Dres
                .FirstOrDefault()?
                .DreCodigo;

            if (string.IsNullOrWhiteSpace(dreCodigoDestino))
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL);

            if (string.IsNullOrWhiteSpace(dreCodigoOrigem))
                throw new NegocioException(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL);

            if (!string.Equals(dreCodigoOrigem, dreCodigoDestino, StringComparison.OrdinalIgnoreCase))
                throw new NegocioException(MensagemNegocio.DRE_IGUAL_ORIGEM_DESTINO, HttpStatusCode.BadRequest);
        }

    }
}
