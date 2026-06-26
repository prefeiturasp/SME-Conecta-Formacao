using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IRepositorioCodafSuplementarLogRemessaConclusao repositorioCodafSuplementarLog) :
        ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar
    {
        public async Task<Resultado<CodafSuplementarArquivoDto>> ExecutarAsync(long codafSuplementarId)
        {
            var dadosBrutos = await repositorioCodafSuplementar.ObterDadosRemessaConclusaoCodafAsync(codafSuplementarId);

            if (dadosBrutos == null)
                return Erro.NaoEncontrado();

            var linhasFormatadas = MapearParaDtoArquivo(dadosBrutos);
            var streamArquivo = GerarStreamArquivoTxt(linhasFormatadas);
            var primeiroRegistro = dadosBrutos.First();
            var nomeArquivo = GerarNomeArquivo(primeiroRegistro.NumeroHomologacao, primeiroRegistro.NomeTurma);

            var hashArquivo = CalcularHashSha256(streamArquivo);

            await RegistrarLogGeracaoAsync(codafSuplementarId, hashArquivo, linhasFormatadas.Count, nomeArquivo);

            return new CodafSuplementarArquivoDto(nomeArquivo, "application/octet-stream", streamArquivo);
        }

        private static MemoryStream GerarStreamArquivoTxt(List<DadosArquivoCodafEolDto> dados)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
            foreach (var dado in dados)
            {
                writer.WriteLine(dado.ToString());
            }

            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static string GerarNomeArquivo(long numeroHomologacao, string nomeTurma)
        {
            var nomeTurmaLimpo = nomeTurma.RemoverCaracteresEspeciais();
            return $"HOM{numeroHomologacao}{nomeTurmaLimpo}.txt";
        }

        private static string CalcularHashSha256(MemoryStream stream)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var posicaoOriginal = stream.Position;

            stream.Position = 0;
            var hashBytes = sha256.ComputeHash(stream);

            stream.Position = posicaoOriginal;

            return Convert.ToHexStringLower(hashBytes);
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
        private static List<DadosArquivoCodafEolDto> MapearParaDtoArquivo(IEnumerable<DadosConsultaParaTxtEolDto> dadosBrutos)
        {
            return [.. dadosBrutos.Select(dado => new DadosArquivoCodafEolDto
            {
                RegistroFuncional = dado.RegistroFuncional,
                CodigoCursoEol = dado.CodigoCursoEol.ToString(),
                CodigoNivel = $"{dado.CodigoNivel:00}",
                DataFimCurso = dado.DataFimCurso?.ToString("dd/MM/yyyy") ?? string.Empty,
                NumeroHomologacao = $"HOM{dado.NumeroHomologacao}",
                CargaHoraria = (dado.HorasTotais ?? dado.CargaHorariaTotalOutra.ConverterHoraMinutoParaInteiro()).ToString("00")
            })];
        }
    }
}
