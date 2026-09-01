using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterAreaPromotoraUsuarioLogadoQuery : IRequest<Dominio.Entidades.AreaPromotora?>
    {
        private static ObterAreaPromotoraUsuarioLogadoQuery? _instancia;
        public static ObterAreaPromotoraUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
