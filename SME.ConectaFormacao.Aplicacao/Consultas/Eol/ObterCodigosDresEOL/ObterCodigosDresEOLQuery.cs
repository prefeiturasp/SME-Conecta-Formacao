using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresEOLQuery : IRequest<IEnumerable<DreServicoEol>>
{
    private static ObterCodigosDresEOLQuery _instancia;
    public static ObterCodigosDresEOLQuery Instancia => _instancia ??= new();
}