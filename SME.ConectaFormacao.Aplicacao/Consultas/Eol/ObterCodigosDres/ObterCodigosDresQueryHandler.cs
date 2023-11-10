using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresQueryHandler : IRequestHandler<ObterCodigosDresQuery, IEnumerable<DreNomeAbreviacaoDTO>>
{
    private readonly IServicoEol _servicoEol;

    public ObterCodigosDresQueryHandler(IServicoEol servicoEol)
    {
        _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
    }

    public async Task<IEnumerable<DreNomeAbreviacaoDTO>> Handle(ObterCodigosDresQuery request, CancellationToken cancellationToken)
    {
        return await _servicoEol.ObterCodigosDres();
    }
}