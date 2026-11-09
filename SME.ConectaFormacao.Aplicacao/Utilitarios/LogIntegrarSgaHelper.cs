using System.Text.Json;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public static class LogIntegrarSgaHelper
    {
        public const string Tag = "[INTEGRA_SGA]";

        public static async Task RegistrarLogSgaAsync(
            IServicoLogs? servicoLogs,
            string operacao,
            long propostaId,
            bool? valorBancoAnterior,
            bool? valorRecebidoDto,
            bool valorFinalPersistido,
            string? nomeFormacao = null,
            long? areaPromotoraId = null,
            string? situacao = null,
            string? usuario = null,
            string? detalhesAdicionais = null)
        {
            if (servicoLogs == null)
                return;

            try
            {
                var mudouDeTrueParaFalse = valorBancoAnterior == true && !valorFinalPersistido;
                var nivel = mudouDeTrueParaFalse ? LogNivel.Alerta : LogNivel.Informacao;

                var decisao = valorRecebidoDto == null
                    ? "DTO enviou NULL/INDEFINIDO -> Valor do banco foi PRESERVADO"
                    : (valorBancoAnterior.HasValue && valorBancoAnterior.Value != valorFinalPersistido)
                        ? $"Valor ALTERADO de {valorBancoAnterior} para {valorFinalPersistido} conforme DTO"
                        : $"Valor DEFINIDO/MANTIDO como {valorFinalPersistido} conforme DTO";

                var bancoStr = valorBancoAnterior.HasValue ? valorBancoAnterior.Value.ToString().ToUpper() : "N/A";
                var dtoStr = valorRecebidoDto.HasValue ? valorRecebidoDto.Value.ToString().ToUpper() : "NULL";
                var finalStr = valorFinalPersistido.ToString().ToUpper();

                var mensagem = $"{Tag} [{operacao}] PropostaId: {propostaId} | Banco: {bancoStr} | DTO: {dtoStr} | Final: {finalStr} | {decisao}";

                var observacaoObj = new
                {
                    tag = Tag,
                    operacao,
                    propostaId,
                    nomeFormacao = nomeFormacao ?? "N/A",
                    areaPromotoraId,
                    situacao = situacao ?? "N/A",
                    valorBancoAnterior,
                    valorRecebidoDTO = valorRecebidoDto,
                    valorFinalPersistido,
                    mudouDeTrueParaFalse,
                    decisao,
                    usuario = usuario ?? "N/A",
                    detalhesAdicionais,
                    dataHoraUtc = DateTime.UtcNow,
                    dataHoraBrasilia = DateTimeExtension.HorarioBrasilia().ToString("yyyy-MM-dd HH:mm:ss")
                };

                var observacaoJson = JsonSerializer.Serialize(observacaoObj, new JsonSerializerOptions { WriteIndented = false });

                await servicoLogs.Enviar(
                    mensagem: mensagem,
                    contexto: LogContexto.Geral,
                    nivel: nivel,
                    observacao: observacaoJson
                );
            }
            catch
            {
                // Logs de monitoramento não devem impactar a operação em caso de falha de conexão com mensageria
            }
        }
    }
}
