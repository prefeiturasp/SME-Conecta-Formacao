using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresEOLQuery : IRequest<IEnumerable<DreNomeAbreviacaoDTO>>
{
    private static ObterCodigosDresEOLQuery _instance;
    public static ObterCodigosDresEOLQuery Instance => _instance ??= new();
}