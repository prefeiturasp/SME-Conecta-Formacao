using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterCodigosDresEOLQueryHandler : IRequestHandler<ObterCodigosDresEOLQuery, IEnumerable<DreServicoEol>>
{
    private readonly IServicoEol _servicoEol;
    private readonly IMediator _mediator;

    public ObterCodigosDresEOLQueryHandler(IServicoEol servicoEol, IMediator mediator)
    {
        _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<DreServicoEol>> Handle(ObterCodigosDresEOLQuery request, CancellationToken cancellationToken)
    {

        try
        {
            return await _servicoEol.ObterCodigosDres();
        }
        catch (Exception ex)
        {
            await _mediator.Send(new SalvarLogCommand(
                "SincronizacaoCargosEol - ObterCodigosDres",
                LogNivel.Negocio,
                ex.InnerException?.Message ?? "",
                ex.InnerException?.StackTrace ?? ""
            ));

            throw;
        }
    }
}