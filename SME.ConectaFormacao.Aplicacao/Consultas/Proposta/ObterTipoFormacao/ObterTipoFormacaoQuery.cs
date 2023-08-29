using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTipoFormacaoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterTipoFormacaoQuery _instancia;
        public static ObterTipoFormacaoQuery Instancia => _instancia ??= new();
    }
}
