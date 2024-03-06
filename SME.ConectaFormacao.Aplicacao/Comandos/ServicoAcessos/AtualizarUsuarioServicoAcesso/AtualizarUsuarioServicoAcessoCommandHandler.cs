using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtualizarUsuarioServicoAcessoCommandHandler : IRequestHandler<AtualizarUsuarioServicoAcessoCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public AtualizarUsuarioServicoAcessoCommandHandler(IServicoAcessos servicoAcessos)
        {
            this._servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(AtualizarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.AtualizarUsuarioCoreSSO(request.Login, request.Nome, request.Email, request.Senha);
        }
    }
}
