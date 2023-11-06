using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterListaDreQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterListaDreQuery _instancia;
        public static ObterListaDreQuery Instancia => _instancia ??= new();
    }
}