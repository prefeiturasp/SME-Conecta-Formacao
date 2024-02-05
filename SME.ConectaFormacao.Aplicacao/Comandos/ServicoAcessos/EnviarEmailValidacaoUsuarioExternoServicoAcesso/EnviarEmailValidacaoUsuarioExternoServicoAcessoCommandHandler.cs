using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandHandler : IRequestHandler<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand, bool>
{
    private readonly IServicoAcessos servicoAcessos;

    public EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandHandler(IServicoAcessos servicoAcessos)
    {
        this.servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
    }

    public Task<bool> Handle(EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand request, CancellationToken cancellationToken)
    {
        return servicoAcessos.EnviarEmailValidacaoUsuarioExterno(request.Login);
    }
}
