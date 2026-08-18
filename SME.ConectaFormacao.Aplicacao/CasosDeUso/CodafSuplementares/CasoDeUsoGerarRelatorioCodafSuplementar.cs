using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoGerarRelatorioCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IGeradorRelatorioCodafSuplementarExcelService geradorRelatorioCodafSuplementarExcelService) :
        ICasoDeUsoGerarRelatorioCodafSuplementar
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafSuplementarId)
        {
            var codafSuplementar = await repositorioCodafSuplementar.ObterNaoExcluidosPorIdAsync(codafSuplementarId);
            if (codafSuplementar == null)
                return Erro.NaoEncontrado("Nenhuma informação encontrada para o codaf informado.");

            var dadosRelatorio = await repositorioCodafSuplementar.ObterDadosRelatorioSuplementarAsync(codafSuplementarId);
            if (dadosRelatorio == null)
                return Erro.NaoEncontrado("Nenhuma informação encontrada para o codaf informado.");
            dadosRelatorio.ObservacaoCodafSuplementar = $"Documento suplementar do arquivo gerado em {dadosRelatorio.DataCodaf:dd/MM/yyyy}";
            var arquivoBytes = geradorRelatorioCodafSuplementarExcelService.GerarRelatorio(dadosRelatorio, ehCodafSuplementar: true);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var nomeArquivo = $"CODAF_SUPLEMENTAR_{dadosRelatorio.NumeroHomologacao}-{dadosRelatorio.NomeTurma}.xlsx";
            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, new MemoryStream(arquivoBytes, writable: false));

            await AtualizarStatusParaFinalizadoAsync(codafSuplementar);
            return arquivoDto;
        }

        private async Task AtualizarStatusParaFinalizadoAsync(CodafSuplementar codafSuplementar)
        {
            if (codafSuplementar.Status == StatusCodafSuplementar.Finalizado)
                return;

            codafSuplementar.Finalizar();
            await repositorioCodafSuplementar.Atualizar(codafSuplementar);
        }
    }
}
