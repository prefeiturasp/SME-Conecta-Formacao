using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGrupoUsuarioEDresUsuarioLogadoQueryHandler : IRequestHandler<ObterGrupoUsuarioEDresUsuarioLogadoQuery, (Guid,IEnumerable<string>)>
    {
        private readonly IContextoAplicacao _contextoAplicacao;

        public ObterGrupoUsuarioEDresUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public Task<(Guid,IEnumerable<string>)> Handle(ObterGrupoUsuarioEDresUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult((new Guid(_contextoAplicacao.PerfilUsuario), _contextoAplicacao.Dres));
        }
    }
}
