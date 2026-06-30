using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IRepositorioCodafSuplementarLogRemessaConclusao repositorioCodafSuplementarLog) :
        ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafSuplementarId)
        {
            var dadosBrutos = await repositorioCodafSuplementar.ObterDadosRemessaConclusaoCodafSuplementarAsync(codafSuplementarId);

            if (dadosBrutos == null)
                return Erro.NaoEncontrado();

            var linhasFormatadas = ProcessadorGeracaoArquivoRemessaConclusao.MapearParaDtoArquivo(dadosBrutos);
            var streamArquivo = ProcessadorGeracaoArquivoRemessaConclusao.GerarStreamArquivoTxt(linhasFormatadas);
            var primeiroRegistro = dadosBrutos.First();
            var nomeArquivo = ProcessadorGeracaoArquivoRemessaConclusao.GerarNomeArquivo(primeiroRegistro.NumeroHomologacao, primeiroRegistro.NomeTurma);

            var hashArquivo = ProcessadorGeracaoArquivoRemessaConclusao.CalcularHashSha256(streamArquivo);

            await RegistrarLogGeracaoAsync(codafSuplementarId, hashArquivo, linhasFormatadas.Count, nomeArquivo);

            return new ArquivoDto(nomeArquivo, "application/octet-stream", streamArquivo);
        }

        private async Task RegistrarLogGeracaoAsync(long codafSuplementarId, string hashArquivo, int quantidadeRegistros, string nomeArquivoGerado)
        {
            var logRemessa = new Dominio.Entidades.CodafSuplementarLogRemessaConclusao
            {
                CodafSuplementarId = codafSuplementarId,
                HashArquivo = hashArquivo,
                QuantidadeRegistros = quantidadeRegistros,
                NomeArquivoGerado = nomeArquivoGerado
            };
            await repositorioCodafSuplementarLog.InserirAsync(logRemessa);
        }
    }
}
