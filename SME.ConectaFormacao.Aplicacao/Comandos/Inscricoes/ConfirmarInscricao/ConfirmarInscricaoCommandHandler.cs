using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ConfirmarInscricao
{
    public class ConfirmarInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao, ITransacao transacao, IMediator mediator) :
        IRequestHandler<ConfirmarInscricaoCommand, bool>
    {
        private readonly ITransacao _transacao = transacao;

        public async Task<bool> Handle(ConfirmarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await repositorioInscricao.ObterPorId(request.Id);
            if (inscricao.EhNulo() || inscricao.Excluido)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA);

            if (inscricao.Situacao.NaoEhAguardandoAnaliseEEmEspera())
                throw new NegocioException(MensagemNegocio.INSCRICAO_SOMENTE_INSCRICAO_AGUARDANDO_ANALISE_OE_EM_ESPERA_PODE_IR_PARA_CONFIRMADA);

            var transacao = _transacao.Iniciar();
            try
            {
                bool confirmada = await repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                if (!confirmada)
                    throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                inscricao.Situacao = SituacaoInscricao.Confirmada;
                await repositorioInscricao.Atualizar(inscricao);
                await mediator.Send(new EnviarEmailConfirmacaoInscricaoCommand(request.Id), cancellationToken);
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
