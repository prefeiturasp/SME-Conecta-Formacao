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
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TransferirInscricaoCommandHandler : IRequestHandler<TransferirInscricaoCommand, RetornoDTO>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;
        private readonly IRepositorioProposta _repositorioProposta;

        public TransferirInscricaoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioInscricao repositorioInscricao, IRepositorioCargoFuncao repositorioCargoFuncao, IMediator mediator, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<RetornoDTO> Handle(TransferirInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterPorId(request.IdInscricao)
                ?? throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            if (inscricao.Situacao == SituacaoInscricao.Cancelada)
                throw new NegocioException(MensagemNegocio.INSCRICOES_CANCELADAS, HttpStatusCode.BadRequest);

            if (request.InscricaoTransferenciaDTO.IdTurmaDestino == inscricao.PropostaTurmaId)
                throw new NegocioException(MensagemNegocio.INSCRICAO_MESMA_TURMA_ORIGEM_DESTINO, HttpStatusCode.BadRequest);

            if (!request.InscricaoTransferenciaDTO.Cursistas.Any() || request.InscricaoTransferenciaDTO.Cursistas == null)
                throw new NegocioException("Cursista(s) não informado(s).", HttpStatusCode.BadRequest);

            var turmaDestino = await _repositorioProposta.ObterTurmasComVagaPorId(request.InscricaoTransferenciaDTO.IdTurmaDestino, null)
                ?? throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            if (inscricao.CargoCodigo != null && inscricao.CargoCodigo != "")
            {
                if (request.InscricaoTransferenciaDTO.CargoCodigo != inscricao.CargoCodigo)
                    throw new NegocioException(MensagemNegocio.INSCRICAO_TRANSFERENCIA_CARGOS_DIFERENTES, HttpStatusCode.BadRequest);
            }

            if (inscricao.FuncaoCodigo != null && inscricao.FuncaoCodigo != "")
            {
                if (request.InscricaoTransferenciaDTO.FuncaoCodigo != inscricao.FuncaoCodigo)
                    throw new NegocioException(MensagemNegocio.INSCRICAO_TRANSFERENCIA_FUNCOES_DIFERENTES, HttpStatusCode.BadRequest);
            }

            if (inscricao.FuncaoDreCodigo != null && inscricao.FuncaoDreCodigo != "")
            {
                if (request.InscricaoTransferenciaDTO.FuncaoDreCodigo != inscricao.FuncaoDreCodigo)
                    throw new NegocioException(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_AUTOMATICO, HttpStatusCode.BadRequest);
            }

            var usuarioLogado = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia(), cancellationToken)
                ?? throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var transacao = _transacao.Iniciar();

            try
            {
                inscricao.Situacao = SituacaoInscricao.Cancelada;
                await _repositorioInscricao.Atualizar(inscricao);
                await _mediator.Send(new EnviarEmailCancelarInscricaoCommand(request.IdInscricao, "Inscrição transferida"), cancellationToken);

                var inscricaoNova = new Inscricao();
                inscricaoNova.Situacao = SituacaoInscricao.Confirmada;
                inscricaoNova.UsuarioId = usuarioLogado.Id;
                inscricaoNova.CargoCodigo = request.InscricaoTransferenciaDTO.CargoCodigo;
                inscricaoNova.FuncaoDreCodigo = request.InscricaoTransferenciaDTO.FuncaoDreCodigo;
                inscricaoNova.FuncaoCodigo = request.InscricaoTransferenciaDTO.FuncaoCodigo;
                inscricaoNova.Origem = OrigemInscricao.Transferencia;
                inscricaoNova.PropostaTurmaId = turmaDestino.FirstOrDefault().Id;
                inscricaoNova.PropostaTurma = turmaDestino.FirstOrDefault();

                await _repositorioInscricao.Inserir(inscricaoNova);

                foreach (var cursista in request.InscricaoTransferenciaDTO.Cursistas)
                {
                    var dto = new InscricaoAutomaticaDTO
                    {
                        PropostaId = inscricao.Id,
                        PropostaTurmaId = request.InscricaoTransferenciaDTO.IdTurmaDestino,

                        CargoId = inscricao.CargoId,
                        CargoCodigo = cursista.CargoCodigo,
                        CargoDreCodigo = cursista.CargoDreCodigo,
                        CargoUeCodigo = cursista.CargoUeCodigo,

                        FuncaoId = inscricao.FuncaoId,
                        FuncaoCodigo = cursista.FuncaoCodigo,
                        FuncaoDreCodigo = cursista.FuncaoDreCodigo,
                        FuncaoUeCodigo = cursista.FuncaoUeCodigo,

                        UsuarioId = inscricao.UsuarioId,
                        UsuarioRf = cursista.Rf,
                        UsuarioNome = cursista.Nome,
                        UsuarioCpf = cursista.Cpf,

                        TipoVinculo = cursista.TipoVinculo
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
    }
}
