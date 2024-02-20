using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao;

public class RemoverCacheCommandHandler : IRequestHandler<RemoverCacheCommand>
{
    private readonly ICacheDistribuido _cacheDistribuido;

    public RemoverCacheCommandHandler(ICacheDistribuido cacheDistribuido)
    {
        _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
    }

    public async Task Handle(RemoverCacheCommand request, CancellationToken cancellationToken)
    {
        await _cacheDistribuido.RemoverAsync(request.Chave);
    }
}