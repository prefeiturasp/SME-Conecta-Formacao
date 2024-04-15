using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao;

public class ValidarResponsavelDfCommandHandler : IRequestHandler<ValidarResponsavelDfCommand>
{
    private readonly IMediator _mediator;

    public ValidarResponsavelDfCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Handle(ValidarResponsavelDfCommand request, CancellationToken cancellationToken)
    {
        var grupoUsuarioLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);
        var ehFormacaoHomologada = request.FormacaoHomologada == FormacaoHomologada.Sim;
        if (grupoUsuarioLogado == Perfis.ADMIN_DF && ehFormacaoHomologada && string.IsNullOrEmpty(request.RfResponsavelDf))
            throw new NegocioException(MensagemNegocio.RESPONSAVEL_DF_DEVE_SER_INFORMADO);
    }
}