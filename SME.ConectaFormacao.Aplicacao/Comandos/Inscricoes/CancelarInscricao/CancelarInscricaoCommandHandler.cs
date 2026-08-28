using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao
{
    public class CancelarInscricaoCommandHandler(ITransacao transacao, IRepositorioInscricao repositorioInscricao, IMediator mediator) : IRequestHandler<CancelarInscricaoCommand, bool>
    {
        public async Task<bool> Handle(CancelarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await repositorioInscricao.ObterNaoExcluidosPorIdAsync(request.Id);

            if (inscricao == null) return true;

            using var transacaoDb = transacao.Iniciar();
            try
            {
                if (inscricao.Situacao == SituacaoInscricao.Confirmada)
                    await repositorioInscricao.LiberarInscricaoVaga(inscricao);

                inscricao.SituacaoAnterior = inscricao.Situacao;
                inscricao.Situacao = SituacaoInscricao.Cancelada;

                if (!string.IsNullOrWhiteSpace(request.Motivo))
                    inscricao.MotivoCancelamento = request.Motivo;

                await repositorioInscricao.Atualizar(inscricao);
                transacaoDb.Commit();
                await mediator.Send(new EnviarEmailCancelarInscricaoCommand(request.Id, inscricao.MotivoCancelamento), cancellationToken);

                return true;
            }
            catch
            {
                transacaoDb.Rollback();
                throw;
            }
        }
    }
}
