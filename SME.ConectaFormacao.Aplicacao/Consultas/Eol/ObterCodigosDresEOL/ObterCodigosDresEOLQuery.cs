using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresEOLQuery : IRequest<IEnumerable<DreNomeAbreviacaoDTO>>
{
    private static ObterCodigosDresEOLQuery _instancia;
    public static ObterCodigosDresEOLQuery Instancia => _instancia ??= new();
}