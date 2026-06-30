using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoGerarArquivoRemessaConclusaoCodaf(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca, 
        IRepositorioCodafLogRemessaConclusao repositorioCodafLog) : 
        ICasoDeUsoGerarArquivoRemessaConclusaoCodaf
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafListaPresencaId)
        {
            var dadosBrutos = await repositorioCodafListaPresenca.ObterDadosRemessaConclusaoCodafAsync(codafListaPresencaId);

            if (dadosBrutos == null || !dadosBrutos.Any())
                return Erro.NaoEncontrado();

            var linhasFormatadas = ProcessadorGeracaoArquivoRemessaConclusao.MapearParaDtoArquivo(dadosBrutos);
            var streamArquivo = ProcessadorGeracaoArquivoRemessaConclusao.GerarStreamArquivoTxt(linhasFormatadas);
            var primeiroRegistro = dadosBrutos.First();
            var nomeArquivo = ProcessadorGeracaoArquivoRemessaConclusao.GerarNomeArquivo(primeiroRegistro.NumeroHomologacao, primeiroRegistro.NomeTurma);

            var hashArquivo = ProcessadorGeracaoArquivoRemessaConclusao.CalcularHashSha256(streamArquivo);

            await RegistrarLogGeracaoAsync(codafListaPresencaId, hashArquivo, linhasFormatadas.Count, nomeArquivo);

            return new ArquivoDto(nomeArquivo, "application/octet-stream", streamArquivo);
        }

        private async Task RegistrarLogGeracaoAsync(long codafListaPresencaId, string hashArquivo, int quantidadeRegistros, string nomeArquivoGerado)
        {
            var logRemessa = new Dominio.Entidades.CodafLogRemessaConclusao
            {
                CodafListaPresencaId = codafListaPresencaId,
                HashArquivo = hashArquivo,
                QuantidadeRegistros = quantidadeRegistros,
                NomeArquivoGerado = nomeArquivoGerado
            };
            await repositorioCodafLog.InserirAsync(logRemessa);
        }
    }
}
