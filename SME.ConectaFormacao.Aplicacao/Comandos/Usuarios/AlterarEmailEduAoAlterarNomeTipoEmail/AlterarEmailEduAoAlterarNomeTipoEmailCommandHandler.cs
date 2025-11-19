using MediatR;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarEmailEduAoAlterarNomeTipoEmailCommandHandler : IRequestHandler<AlterarEmailEduAoAlterarNomeTipoEmailCommand, bool>
{
    private readonly IMediator _mediator;

    public AlterarEmailEduAoAlterarNomeTipoEmailCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(AlterarEmailEduAoAlterarNomeTipoEmailCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _mediator.Send(new ObterUsuarioPorLoginQuery(request.Login));

        usuario.EmailEducacional = await _mediator.Send(new GerarEmailEducacionalCommand(usuario));

        await _mediator.Send(new SalvarUsuarioCommand(usuario, true));

        return true;
    }
}