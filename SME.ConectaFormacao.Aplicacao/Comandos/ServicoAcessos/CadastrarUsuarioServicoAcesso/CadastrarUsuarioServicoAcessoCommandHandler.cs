using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class CadastrarUsuarioServicoAcessoCommandHandler : IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>
{
    private readonly IServicoAcessos servicoAcessos;

    public CadastrarUsuarioServicoAcessoCommandHandler(IServicoAcessos servicoAcessos)
    {
        this.servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
    }

    public async Task<bool> Handle(CadastrarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
    {
        return await servicoAcessos.CadastrarUsuarioCoreSSO(request.Login, request.Nome, request.Email, request.Senha);
    }
}
