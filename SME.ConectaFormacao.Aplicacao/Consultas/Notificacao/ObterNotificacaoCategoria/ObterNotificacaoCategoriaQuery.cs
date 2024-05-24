using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoCategoriaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterNotificacaoCategoriaQuery _instancia;
        public static ObterNotificacaoCategoriaQuery Instancia() => _instancia ??= new();
    }
}
