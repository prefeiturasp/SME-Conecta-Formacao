using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraUsuarioLogadoQuery : IRequest<Dominio.Entidades.AreaPromotora?>
    {
        private static ObterAreaPromotoraUsuarioLogadoQuery? _instancia;
        public static ObterAreaPromotoraUsuarioLogadoQuery Instancia() => _instancia ??= new();
    }
}
