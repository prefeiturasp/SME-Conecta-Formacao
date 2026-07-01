using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
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

            var logRemessa = await RegistrarLogGeracaoAsync(codafSuplementarId, hashArquivo, linhasFormatadas.Count, nomeArquivo);

            var codafSuplementar = await repositorioCodafSuplementar.ObterPorId(codafSuplementarId);
            codafSuplementar.CodafSuplementarLogRemessasConclusao = codafSuplementar.CodafSuplementarLogRemessasConclusao ?? new List<CodafSuplementarLogRemessaConclusao>();
            codafSuplementar.CodafSuplementarLogRemessasConclusao.Add(logRemessa);
            codafSuplementar.DefinirStatus();
            await repositorioCodafSuplementar.Atualizar(codafSuplementar);

            return new ArquivoDto(nomeArquivo, "application/octet-stream", streamArquivo);
        }

        private async Task<CodafSuplementarLogRemessaConclusao> RegistrarLogGeracaoAsync(long codafSuplementarId, string hashArquivo, int quantidadeRegistros, string nomeArquivoGerado)
        {
            var logRemessa = new CodafSuplementarLogRemessaConclusao
            {
                CodafSuplementarId = codafSuplementarId,
                HashArquivo = hashArquivo,
                QuantidadeRegistros = quantidadeRegistros,
                NomeArquivoGerado = nomeArquivoGerado
            };
            await repositorioCodafSuplementarLog.InserirAsync(logRemessa);
            return logRemessa;
        }
    }
}
