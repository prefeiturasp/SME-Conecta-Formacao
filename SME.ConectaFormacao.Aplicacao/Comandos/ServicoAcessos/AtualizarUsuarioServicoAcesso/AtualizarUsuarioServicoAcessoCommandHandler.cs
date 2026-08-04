using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtualizarUsuarioServicoAcessoCommandHandler(IServicoAcessos servicoAcessos) : 
        IRequestHandler<AtualizarUsuarioServicoAcessoCommand, bool>
    {
        public Task<bool> Handle(AtualizarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return servicoAcessos.AtualizarUsuarioCoreSSO(request.Login, request.Nome, request.Email, request.Senha, request.NomeSocial);
        }
    }
}
