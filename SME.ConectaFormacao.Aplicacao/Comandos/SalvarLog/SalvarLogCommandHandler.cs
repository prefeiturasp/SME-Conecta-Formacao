using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
namespace SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;

public class SalvarLogCommandHandler(
    IRepositorioLog repositorio,
    IMapper mapper,
    IMediator mediator,
    IServicoLogs servicoLogs) :
    IRequestHandler<SalvarLogCommand, bool>
{

    public async Task<bool> Handle(SalvarLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);

            usuarioLogado ??= new Usuario
            {
                Id = 1,
                Login = "Sistema",
            };


            var log = mapper.Map<Log>(request);
            log.CriadoPor = usuarioLogado.Id.ToString();
            log.CriadoLogin = usuarioLogado.Login;
            log.CriadoEm = DateTime.Now;
            log.Mensagem = $"[{request.IdentificadorRastreamento}] {request.Mensagem}";


            var complemento = MontarComplementoExcecao(request.Excecao);
            log.Complemento = string.IsNullOrEmpty(request.Complemento) ? complemento : $"{request.Complemento} | {complemento}";

            await repositorio.InserirAsync(log);

            if (request.Excecao is null)
                await servicoLogs.Enviar(mensagem: log.Mensagem, nivel: log.NivelLog);
            else
                await servicoLogs.Enviar(request.Excecao, mensagem: log.Mensagem, nivel: log.NivelLog);

            return true;
        }
        catch (Exception ex)
        {
            await servicoLogs.Enviar(ex, mensagem: $"Erro ao salvar log: {request.Mensagem}", nivel: LogNivel.Critico);
            return false;
        }
    }

    private static string MontarComplementoExcecao(Exception? ex, int nivel = 0)
    {
        var complemento = string.Empty;
        if (ex is not null)
            complemento = $"{nivel + 1}: Exception: {ex.Message} | StackTrace: {ex.StackTrace}";

        if (ex is not null && ex.InnerException is not null)
            complemento += Environment.NewLine + MontarComplementoExcecao(ex.InnerException, nivel + 1);

        return complemento;
    }
}