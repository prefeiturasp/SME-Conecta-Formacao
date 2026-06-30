using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.Utilitarios
{
    public class ProcessadorGeracaoArquivoRemessaConclusaoTestes
    {
        [Fact]
        public void Deve_mapear_utilizando_horas_totais()
        {
            var dados = new List<DadosConsultaParaTxtEolDto>
            {
                new()
                {
                    RegistroFuncional = "123456",
                    CodigoCursoEol = 987,
                    CodigoNivel = 2,
                    DataFimCurso = new DateTime(2024, 12, 20, 0, 0, 0, DateTimeKind.Unspecified),
                    NumeroHomologacao = 100,
                    HorasTotais = 40,
                    CargaHorariaTotalOutra = "20:00",
                    NomeTurma = "Turma Teste"
                }
            };

            var resultado = ProcessadorGeracaoArquivoRemessaConclusao.MapearParaDtoArquivo(dados);

            Assert.Single(resultado);

            var dto = resultado[0];

            Assert.Equal("123456", dto.RegistroFuncional);
            Assert.Equal("987", dto.CodigoCursoEol);
            Assert.Equal("02", dto.CodigoNivel);
            Assert.Equal("20/12/2024", dto.DataFimCurso);
            Assert.Equal("HOM100", dto.NumeroHomologacao);
            Assert.Equal("40", dto.CargaHoraria);
        }

        [Fact]
        public void Deve_mapear_utilizando_carga_horaria_total_outra_quando_horas_totais_for_nulo()
        {
            var dados = new List<DadosConsultaParaTxtEolDto>
            {
                new() {
                    RegistroFuncional = "654321",
                    CodigoCursoEol = 10,
                    CodigoNivel = 1,
                    DataFimCurso = null,
                    NumeroHomologacao = 999,
                    HorasTotais = null,
                    CargaHorariaTotalOutra = "08:30",
                    NomeTurma = "Turma Teste"
                }
            };

            var resultado = ProcessadorGeracaoArquivoRemessaConclusao.MapearParaDtoArquivo(dados);

            var dto = resultado.Single();

            Assert.Equal(string.Empty, dto.DataFimCurso);
            Assert.Equal("08", dto.CargaHoraria);
        }

        [Fact]
        public void Deve_gerar_stream_txt()
        {
            var dados = new List<DadosArquivoCodafEolDto>
            {
                new()
                {
                    RegistroFuncional = "1",
                    CodigoCursoEol = "2",
                    CodigoNivel = "03",
                    DataFimCurso = "01/01/2024",
                    NumeroHomologacao = "HOM10",
                    CargaHoraria = "40"
                },
                new()
                {
                    RegistroFuncional = "5",
                    CodigoCursoEol = "6",
                    CodigoNivel = "02",
                    DataFimCurso = "02/02/2024",
                    NumeroHomologacao = "HOM20",
                    CargaHoraria = "20"
                }
            };

            var stream = ProcessadorGeracaoArquivoRemessaConclusao.GerarStreamArquivoTxt(dados);

            Assert.Equal(0, stream.Position);

            using var reader = new StreamReader(stream, Encoding.UTF8);

            var texto = reader.ReadToEnd();

            Assert.Contains(dados[0].ToString(), texto);
            Assert.Contains(dados[1].ToString(), texto);
        }

        [Theory]
        [InlineData(123, "Turma Árvore 2024!", "HOM123TurmaArvore2024.txt")]
        [InlineData(1, "Turma Teste", "HOM1TurmaTeste.txt")]
        public void Deve_gerar_nome_arquivo(long homologacao, string turma, string esperado)
        {
            var resultado = ProcessadorGeracaoArquivoRemessaConclusao.GerarNomeArquivo(homologacao, turma);

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void Deve_calcular_hash_sha256_e_restaurar_posicao_stream()
        {
            var bytes = Encoding.UTF8.GetBytes("Conteúdo para hash");
            var stream = new MemoryStream(bytes)
            {
                Position = 5
            };

            var hash = ProcessadorGeracaoArquivoRemessaConclusao.CalcularHashSha256(stream);

            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.Equal(64, hash.Length);
            Assert.Equal(5, stream.Position);
        }

        [Fact]
        public void Deve_retornar_mesmo_hash_para_mesmo_conteudo()
        {
            var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("teste"));
            var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("teste"));

            var hash1 = ProcessadorGeracaoArquivoRemessaConclusao.CalcularHashSha256(stream1);
            var hash2 = ProcessadorGeracaoArquivoRemessaConclusao.CalcularHashSha256(stream2);

            Assert.Equal(hash1, hash2);
        }
    }
}
