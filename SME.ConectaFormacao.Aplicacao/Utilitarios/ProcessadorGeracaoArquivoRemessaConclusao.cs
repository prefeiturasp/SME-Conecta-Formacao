using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public static class ProcessadorGeracaoArquivoRemessaConclusao
    {
        public static List<DadosArquivoCodafEolDto> MapearParaDtoArquivo(IEnumerable<DadosConsultaParaTxtEolDto> dadosBrutos)
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

        public static MemoryStream GerarStreamArquivoTxt(List<DadosArquivoCodafEolDto> dados)
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

        public static string GerarNomeArquivo(long numeroHomologacao, string nomeTurma)
        {
            var nomeTurmaLimpo = nomeTurma.RemoverCaracteresEspeciais().Replace(" ", "");
            return $"HOM{numeroHomologacao}{nomeTurmaLimpo}.txt";
        }

        public static string CalcularHashSha256(MemoryStream stream)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var posicaoOriginal = stream.Position;

            stream.Position = 0;
            var hashBytes = sha256.ComputeHash(stream);

            stream.Position = posicaoOriginal;

            return Convert.ToHexStringLower(hashBytes);
        }
    }
}