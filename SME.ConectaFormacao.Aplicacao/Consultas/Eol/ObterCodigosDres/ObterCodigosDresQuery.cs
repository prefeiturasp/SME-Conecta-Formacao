using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresQuery : IRequest<IEnumerable<DreNomeAbreviacaoDTO>>
{
    private static ObterCodigosDresQuery _instance;
    public static ObterCodigosDresQuery Instance => _instance ??= new();
}