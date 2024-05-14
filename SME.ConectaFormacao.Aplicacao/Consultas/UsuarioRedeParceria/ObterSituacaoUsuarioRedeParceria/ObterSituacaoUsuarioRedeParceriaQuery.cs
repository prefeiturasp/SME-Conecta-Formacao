using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterSituacaoUsuarioRedeParceriaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterSituacaoUsuarioRedeParceriaQuery _instancia;
        public static ObterSituacaoUsuarioRedeParceriaQuery Instancia() => _instancia ??= new();
    }
}
