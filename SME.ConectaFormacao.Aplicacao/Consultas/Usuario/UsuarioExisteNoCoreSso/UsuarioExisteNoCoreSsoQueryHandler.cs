using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class UsuarioExisteNoCoreSsoQueryHandler : IRequestHandler<UsuarioExisteNoCoreSsoQuery,bool>
    {
        private readonly IServicoAcessos servicoAcessos;

        public UsuarioExisteNoCoreSsoQueryHandler(IServicoAcessos servicoAcessos)
        {
            this.servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<bool> Handle(UsuarioExisteNoCoreSsoQuery request, CancellationToken cancellationToken)
        {
            return await servicoAcessos.UsuarioCadastradoCoreSSO(request.Cpf);
        }
    }
}
