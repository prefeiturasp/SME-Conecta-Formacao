using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTipoInscricaoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterTipoInscricaoQuery _instancia;
        public static ObterTipoInscricaoQuery Instancia => _instancia ??= new();
    }
}
