using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoTipoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterNotificacaoTipoQuery _instancia;
        public static ObterNotificacaoTipoQuery Instancia() => _instancia ??= new();
    }
}
