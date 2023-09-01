using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGrupoUsuarioLogadoQueryHandler : IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>
    {
        private IContextoAplicacao _contextoAplicacao;

        public ObterGrupoUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public Task<Guid> Handle(ObterGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Guid(_contextoAplicacao.PerfilUsuario));
        }
    }
}
