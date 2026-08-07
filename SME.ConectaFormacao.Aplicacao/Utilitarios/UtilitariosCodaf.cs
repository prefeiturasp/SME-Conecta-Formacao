using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public class UtilitariosCodaf : IUtilitariosCodaf
    {
        private readonly IMediator _mediator;
        private readonly IServicoLogs _servicoLogs;
        private readonly string _identificadorRastreamento;

        public UtilitariosCodaf(IMediator mediator, IServicoLogs servicoLogs)
        {
            _mediator = mediator;
            _servicoLogs = servicoLogs;
            _identificadorRastreamento = Guid.NewGuid().ToString();
        }       

        public async Task EnviarEmailsAsync(List<EnviarEmailDto> notificacoesParaEnviar)
        {
            foreach (var emailDto in notificacoesParaEnviar)
            {
                _ = _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, emailDto));
            }
        }

        public async Task SalvarLogAsync(string mensagem, LogNivel nivelLog = LogNivel.Informacao, Exception? ex = null)
        {
            try
            {
                if (ex is null)
                    await _servicoLogs.Enviar(mensagem: $"[{_identificadorRastreamento}] {mensagem}", nivel: nivelLog);
                else
                    await _servicoLogs.Enviar(ex, mensagem: $"[{_identificadorRastreamento}] {mensagem}");

                var complemento = MontarComplementoExcecao(ex);
                await _mediator.Send(new SalvarLogCommand(
                    entidade: typeof(CasoDeUsoGerarArquivoDeclaracoesCodaf).FullName!,
                    nivelLog: nivelLog, mensagem: $"[{_identificadorRastreamento}] {mensagem}", complemento: complemento));
            }
            catch (Exception e)
            {
                await _servicoLogs.Enviar(e, mensagem: $"[{_identificadorRastreamento}] Erro ao salvar log: {mensagem}");
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

        public static TipoEstrategiaCodaf DefinirEstrategia(DadosProcessamentoCodafDto declaracao)
        {
            if (declaracao.TipoParticipacao == TipoParticipacaoCodaf.Regente)
                return TipoEstrategiaCodaf.RegenteComRf;

            return declaracao.TemRf ? TipoEstrategiaCodaf.CursistaComRf : TipoEstrategiaCodaf.CursistaSemRf;
        }
    }
}
