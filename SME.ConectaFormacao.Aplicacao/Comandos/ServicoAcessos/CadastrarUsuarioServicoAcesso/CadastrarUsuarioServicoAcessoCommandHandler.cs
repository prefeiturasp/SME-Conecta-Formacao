using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class CadastrarUsuarioServicoAcessoCommandHandler(IServicoAcessos servicoAcessos) : IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>
{
    public async Task<bool> Handle(CadastrarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
    {
        return await servicoAcessos.CadastrarUsuarioCoreSSO(request.Login, request.Nome, request.Email, request.Senha, request.NomeSocial);
    }
}
