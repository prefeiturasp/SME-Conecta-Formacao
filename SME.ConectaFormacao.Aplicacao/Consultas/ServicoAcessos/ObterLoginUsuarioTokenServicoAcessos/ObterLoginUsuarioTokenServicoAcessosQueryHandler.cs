using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterLoginUsuarioTokenServicoAcessosQueryHandler : IRequestHandler<ObterLoginUsuarioTokenServicoAcessosQuery, string>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public ObterLoginUsuarioTokenServicoAcessosQueryHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<string> Handle(ObterLoginUsuarioTokenServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return await _servicoAcessos.ObterLoginUsuarioToken(request.Token, request.TipoAcao);
        }
    }
}
