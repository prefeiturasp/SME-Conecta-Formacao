using MediatR;
using SME.ConectaFormacao.Infra.Dados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Behaviors
{
    [ExcludeFromCodeCoverage]
    public class TransacaoBehavior<TRequest, TResponse>(ITransacao transacao) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Se não for um Command (ex: Query), não usa transação
            if (!typeof(TRequest).Name.EndsWith("Command"))
                return await next(cancellationToken);

            var transacaoDb = transacao.Iniciar();
            try
            {
                var response = await next(cancellationToken);
                transacaoDb.Commit();
                return response;
            }
            catch
            {
                transacaoDb.Rollback();
                throw;
            }
            finally
            {
                transacaoDb.Dispose();
            }
        }
    }
}
