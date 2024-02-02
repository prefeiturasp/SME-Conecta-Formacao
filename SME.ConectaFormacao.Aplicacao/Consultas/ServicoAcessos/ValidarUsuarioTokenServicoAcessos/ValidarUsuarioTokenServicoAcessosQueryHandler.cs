using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarUsuarioTokenServicoAcessosQueryHandler : IRequestHandler<ValidarUsuarioTokenServicoAcessosQuery, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public ValidarUsuarioTokenServicoAcessosQueryHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(ValidarUsuarioTokenServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            var ehTokenValido = _servicoAcessos.ValidarUsuarioToken(request.Token);
            
        }
    }
}
