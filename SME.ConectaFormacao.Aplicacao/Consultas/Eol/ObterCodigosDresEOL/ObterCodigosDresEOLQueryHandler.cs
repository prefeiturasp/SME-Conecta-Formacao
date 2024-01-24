using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresEOLQueryHandler : IRequestHandler<ObterCodigosDresEOLQuery, IEnumerable<DreServicoEol>>
{
    private readonly IServicoEol _servicoEol;

    public ObterCodigosDresEOLQueryHandler(IServicoEol servicoEol)
    {
        _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
    }

    public async Task<IEnumerable<DreServicoEol>> Handle(ObterCodigosDresEOLQuery request, CancellationToken cancellationToken)
    {
        return await _servicoEol.ObterCodigosDres();
    }
}