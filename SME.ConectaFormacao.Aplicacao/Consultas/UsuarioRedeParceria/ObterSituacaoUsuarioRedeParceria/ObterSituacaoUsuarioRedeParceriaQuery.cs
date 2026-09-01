using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterSituacaoUsuarioRedeParceriaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterSituacaoUsuarioRedeParceriaQuery _instancia;
        public static ObterSituacaoUsuarioRedeParceriaQuery Instancia() => _instancia ??= new();
    }
}
