using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public sealed class LogIntegrarSgaParametros
    {
        public string Operacao { get; init; } = string.Empty;
        public long PropostaId { get; init; }
        public bool? ValorBancoAnterior { get; init; }
        public bool? ValorRecebidoDto { get; init; }
        public bool ValorFinalPersistido { get; init; }
        public string? NomeFormacao { get; init; }
        public long? AreaPromotoraId { get; init; }
        public string? Situacao { get; init; }
        public string? Usuario { get; init; }
        public string? DetalhesAdicionais { get; init; }
    }

    public static class LogIntegrarSgaHelper
    {
        public const string Tag = "[INTEGRA_SGA]";

        public static async Task RegistrarLogSgaAsync(
            IServicoLogs? servicoLogs,
            LogIntegrarSgaParametros parametros)
        {
            if (servicoLogs == null)
                return;

            try
            {
                var mudouDeTrueParaFalse = parametros.ValorBancoAnterior == true && !parametros.ValorFinalPersistido;
                var nivel = mudouDeTrueParaFalse ? LogNivel.Alerta : LogNivel.Informacao;

                string decisao;
                if (parametros.ValorRecebidoDto == null)
                    decisao = "DTO enviou NULL/INDEFINIDO -> Valor do banco foi PRESERVADO";
                else if (parametros.ValorBancoAnterior.HasValue && parametros.ValorBancoAnterior.Value != parametros.ValorFinalPersistido)
                    decisao = $"Valor ALTERADO de {parametros.ValorBancoAnterior} para {parametros.ValorFinalPersistido} conforme DTO";
                else
                    decisao = $"Valor DEFINIDO/MANTIDO como {parametros.ValorFinalPersistido} conforme DTO";

                var bancoStr = parametros.ValorBancoAnterior.HasValue ? parametros.ValorBancoAnterior.Value.ToString().ToUpper() : "N/A";
                var dtoStr = parametros.ValorRecebidoDto.HasValue ? parametros.ValorRecebidoDto.Value.ToString().ToUpper() : "NULL";
                var finalStr = parametros.ValorFinalPersistido.ToString().ToUpper();

                var mensagem = $"{Tag} [{parametros.Operacao}] PropostaId: {parametros.PropostaId} | Banco: {bancoStr} | DTO: {dtoStr} | Final: {finalStr} | {decisao}";

                var observacaoObj = new
                {
                    tag = Tag,
                    operacao = parametros.Operacao,
                    propostaId = parametros.PropostaId,
                    nomeFormacao = parametros.NomeFormacao ?? "N/A",
                    areaPromotoraId = parametros.AreaPromotoraId,
                    situacao = parametros.Situacao ?? "N/A",
                    valorBancoAnterior = parametros.ValorBancoAnterior,
                    valorRecebidoDTO = parametros.ValorRecebidoDto,
                    valorFinalPersistido = parametros.ValorFinalPersistido,
                    mudouDeTrueParaFalse,
                    decisao,
                    usuario = parametros.Usuario ?? "N/A",
                    detalhesAdicionais = parametros.DetalhesAdicionais,
                    dataHoraUtc = DateTime.UtcNow,
                    dataHoraBrasilia = DateTimeExtension.HorarioBrasilia().ToString("yyyy-MM-dd HH:mm:ss")
                };

                var observacaoJson = observacaoObj.ObjetoParaJson();

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
