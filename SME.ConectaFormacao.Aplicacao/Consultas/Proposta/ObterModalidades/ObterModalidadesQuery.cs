using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterModalidadesQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterModalidadesQuery _instancia;
        public static ObterModalidadesQuery Instancia => _instancia ??= new();
    }
}
