using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol(IRepositorioCodafListaPresenca repositorioCodafListaPresenca) : ICasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafListaPresencaId)
        {
            var dadosBrutos = await repositorioCodafListaPresenca.ObterDadosInscritosCodafParaEolPorIdAsync(codafListaPresencaId);

            if (dadosBrutos == null || !dadosBrutos.Any())
                return Erro.NaoEncontrado();

            var linhasFormatadas = dadosBrutos.Select(dado => new DadosArquivoCodafEolDto
            {
                RegistroFuncional = dado.RegistroFuncional,
                CodigoCursoEol = dado.CodigoCursoEol.ToString(),
                CodigoNivel = dado.CodigoNivel.ToString("00"),
                DataFimCurso = dado.DataFimCurso?.ToString("dd/MM/yyyy") ?? string.Empty,
                NumeroHomologacao = $"HOM{dado.NumeroHomologacao}",
                CargaHoraria = (dado.HorasTotais ?? dado.CargaHorariaTotalOutra.ConverterHoraMinutoParaInteiro()).ToString("00")
            }).ToList(); 
            
            var streamArquivo = GerarStreamArquivoTxt(linhasFormatadas); 
            var primeiroRegistro = dadosBrutos.First();
            var nomeArquivo = GerarNomeArquivo(primeiroRegistro.NumeroHomologacao, primeiroRegistro.NomeTurma);

            return new ArquivoDto(nomeArquivo, "application/octet-stream", streamArquivo);
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
    }
}
