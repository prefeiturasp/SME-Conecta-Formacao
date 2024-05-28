using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ConfirmarInscricaoCommandHandler : IRequestHandler<ConfirmarInscricaoCommand, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;


        public ConfirmarInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao, ITransacao transacao, IMediator mediator)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        }

        public async Task<bool> Handle(ConfirmarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterPorId(request.Id);
            if (inscricao.EhNulo() || inscricao.Excluido)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA);

            if (inscricao.Situacao.NaoEhAguardandoAnalise())
                throw new NegocioException(MensagemNegocio.INSCRICAO_SOMENTE_INSCRICAO_AGUARDANDO_ANALISE_OE_EM_ESPERA_PODE_IR_PARA_CONFIRMADA);

            var transacao = _transacao.Iniciar();
            try
            {
                bool confirmada = await _repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                if (!confirmada)
                    throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                inscricao.Situacao = SituacaoInscricao.Confirmada;
                await _repositorioInscricao.Atualizar(inscricao);
                await _mediator.Send(new EnviarEmailConfirmacaoInscricaoCommand(request.Id), cancellationToken);
                transacao.Commit();

                return true;
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
