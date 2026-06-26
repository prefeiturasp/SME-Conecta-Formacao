using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;
using SME.ConectaFormacao.Dominio.Entidades;
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
        _mediator = mediator;
    }

    public async Task<IEnumerable<DreServicoEol>> Handle(ObterCodigosDresEOLQuery request, CancellationToken cancellationToken)
    {
        Usuario usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery());

        if (usuarioLogado is null)
        {
            usuarioLogado = new Usuario
            {
                Id = 1,
                Login = "Sistema",
            };
        }

        try
        {
            return await _servicoEol.ObterCodigosDres();
        }
        catch (Exception ex)
        {
            await _mediator.Send(new SalvarLogCommand(new LogDTO
            {
                CriadoPor = usuarioLogado.Id.ToString(),
                CriadoLogin = usuarioLogado.Login,
                CriadoEm = DateTime.Now,
                Entidade = "SincronizacaoCargosEol - ObterCodigosDres",
                NivelLog = LogNivel.Negocio,
                Mensagem = ex.InnerException?.Message ?? "",
                Complemento = ex.InnerException?.StackTrace ?? ""
            }));

            throw;
        }
    }
}