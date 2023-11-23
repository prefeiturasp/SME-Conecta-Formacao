using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioLogadoQuery : IRequest<Usuario>
    {
        private static ObterUsuarioLogadoQuery _instancia;

        public static ObterUsuarioLogadoQuery Instancia => _instancia ??= new();
    }
}