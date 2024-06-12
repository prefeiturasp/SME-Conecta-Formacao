using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoSituacaoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterNotificacaoSituacaoQuery _instancia;
        public static ObterNotificacaoSituacaoQuery Instancia() => _instancia ??= new();
    }
}
