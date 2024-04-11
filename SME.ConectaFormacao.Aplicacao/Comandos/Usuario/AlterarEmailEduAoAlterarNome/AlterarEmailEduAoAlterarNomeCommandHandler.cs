using MediatR;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarEmailEduAoAlterarNomeCommandHandler : IRequestHandler<AlterarEmailEduAoAlterarNomeCommand, bool>
{
    private readonly IMediator _mediator;

    public AlterarEmailEduAoAlterarNomeCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(AlterarEmailEduAoAlterarNomeCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _mediator.Send(new ObterUsuarioPorLoginQuery(request.Login));
        var emailEdu = await _mediator.Send(new GerarEmailEducacionalCommand(usuario));
        usuario.EmailEducacional = emailEdu;
        await _mediator.Send(new SalvarUsuarioCommand(usuario));
        return true;
    }
}