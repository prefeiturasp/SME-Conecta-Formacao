using Bogus;
using FluentAssertions;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Dtos
{
    public class RelatorioCodafDtoTestes
    {
        private readonly Faker _faker;

        public RelatorioCodafDtoTestes()
        {
            _faker = new Faker();
        }

        [Fact]
        public void DadoDadosValidos_QuandoMapearParaDtoEstruturado_EntaoDeveRetornarDtoMapeadoCorretamente()
        {
            // Arrange
            var dadosBrutos = new DadosPrincipaisRelatorioCodafDto
            {
                TipoFormato = Formato.Hibrido,
                NomeTurma = "Turma Alpha",
                NomeAreaPromotora = "DRE-SA",
                TipoFormacao = TipoFormacao.Curso,
                NomeFormacao = "Formação de Professores",
                QuantidadeTurmas = 2,
                PeriodoRealizacaoInicio = new DateTime(2023, 1, 10),
                PeriodoRealizacaoFim = new DateTime(2023, 1, 15),
                CursoComCertificado = true,
                NumeroHomologacao = 12345,
                CodigoEventoSigpec = 1,
                CargaHorariaTotal = 40,
                CargaHorariaDistancia = "10:00",
                CargaHorariaPresencial = "20:00",
                CargaHorariaSincrona = "10:00",
                NumeroComunicado = 99,
                DataPublicacao = new DateTime(2023, 1, 5),
                DataPublicacaoDom = new DateTime(2023, 1, 6),
                PaginaComunicadoDom = 10,
                QuantidadeVagasTurma = 30,
                NomeDre = "DRE Campo Limpo",
                Observacao = "Obs geral",
                ObservacaoCodafSuplementar = "Obs codaf suplementar",
                DataCodaf = new DateTime(2023, 2, 1),
                DataAulas = new List<DataAulaTurmaRelatorioCodafDto>
                {
                    new DataAulaTurmaRelatorioCodafDto { DataInicio = new DateTime(2023, 1, 11), DataFim = new DateTime(2023, 1, 11) },
                    new DataAulaTurmaRelatorioCodafDto { DataInicio = new DateTime(2023, 1, 12), DataFim = new DateTime(2023, 1, 13) }
                },
                Retificacoes = new List<DadosRetificacaoRelatorioCodafDto>
                {
                    new DadosRetificacaoRelatorioCodafDto { Data = new DateTime(2023, 1, 7), Pagina = 12 }
                },
                Participantes = new List<DadosParticipanteRelatorioCodafDto>
                {
                    new DadosParticipanteRelatorioCodafDto { TemRf = true, Aprovado = true, Nome = "Aluno 1", Documento = "RF1", PercentualFrequencia = 100, AtividadeObrigatoria = true, ConceitoFinal = "S", CodigoCertificado = 1L },
                    new DadosParticipanteRelatorioCodafDto { TemRf = false, Aprovado = true, Nome = "Aluno 2", Documento = "CPF1", PercentualFrequencia = 90, AtividadeObrigatoria = true, ConceitoFinal = "S", CodigoCertificado = 2L },
                    new DadosParticipanteRelatorioCodafDto { TemRf = true, Aprovado = false, Nome = "Aluno 3", Documento = "RF2", PercentualFrequencia = 50, AtividadeObrigatoria = false, ConceitoFinal = "NS", CodigoCertificado = 0L },
                    new DadosParticipanteRelatorioCodafDto { TemRf = false, Aprovado = false, Nome = "Aluno 4", Documento = "CPF2", PercentualFrequencia = 40, AtividadeObrigatoria = false, ConceitoFinal = "NS", CodigoCertificado = 0L }
                },
                RegentesTurma = new List<DadosRegenteTurmaRelatorioCodafDto>
                {
                    new DadosRegenteTurmaRelatorioCodafDto { Nome = "Regente 1", RegistroFuncional = "RF-REG1", CodigoCertificado = "CERT-REG1" }
                }
            };

            // Act
            var resultado = RelatorioCodafDto.MapearParaDtoEstruturado(dadosBrutos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Turmas.Should().HaveCount(1);
            
            var turma = resultado.Turmas.First();
            turma.NomeTurma.Should().Be("Turma Alpha");

            // Asserções Cabeçalho
            turma.Cabecalho.Modalidade.Should().Be(ModalidadeRelatorioCodaf.Hibrido);
            turma.Cabecalho.CargaHorariaDistancia.Should().Be(10);
            turma.Cabecalho.CargaHorariaPresencial.Should().Be(30); // 20 + 10 (Síncrona)
            turma.Cabecalho.PreviaInscritosSme?.TotalInscritos.Should().Be(2); // 2 com RF
            turma.Cabecalho.PreviaInscritosSme?.TotalAprovados.Should().Be(1);
            turma.Cabecalho.PreviaInscritosSme?.TotalReprovados.Should().Be(1);
            
            turma.Cabecalho.PreviaInscritosSemRf?.TotalInscritos.Should().Be(2); // 2 sem RF
            turma.Cabecalho.PreviaInscritosSemRf?.TotalAprovados.Should().Be(1);
            turma.Cabecalho.PreviaInscritosSemRf?.TotalReprovados.Should().Be(1);

            turma.Cabecalho.DataDasAulasSincronas.Should().HaveCount(3); // 11, 12, 13 (Assuming not weekends or handles weekends properly depending on 2023 calendar)

            // Asserções Alunos
            turma.AlunosAprovadosMunicipal?.Alunos.Should().HaveCount(1);
            turma.AlunosAprovadosMunicipal?.Alunos.First().NomeAluno.Should().Be("Aluno 1");
            turma.AlunosAprovadosParceira?.Alunos.Should().HaveCount(1);
            turma.AlunosAprovadosParceira?.Alunos.First().NomeAluno.Should().Be("Aluno 2");
            turma.AlunosReprovadosMunicipal?.Alunos.Should().HaveCount(1);
            turma.AlunosReprovadosMunicipal?.Alunos.First().NomeAluno.Should().Be("Aluno 3");
            turma.AlunosReprovadosParceira?.Alunos.Should().HaveCount(1);
            turma.AlunosReprovadosParceira?.Alunos.First().NomeAluno.Should().Be("Aluno 4");

            // Asserções Regentes
            turma.RegentesDaTurma.Should().HaveCount(1);
            turma.RegentesDaTurma.First().NomeRegente.Should().Be("Regente 1");
        }

        [Fact]
        public void DadoDadosBrutosSemParticipantesNemRegentes_QuandoMapearParaDtoEstruturado_EntaoNaoDeveLancarExcecaoEMapearVazio()
        {
            // Arrange
            var dadosBrutos = new DadosPrincipaisRelatorioCodafDto
            {
                TipoFormato = Formato.Presencial,
                NomeTurma = "Turma Beta",
                Participantes = [],
                RegentesTurma = [],
                DataAulas = []
            };

            // Act
            var resultado = RelatorioCodafDto.MapearParaDtoEstruturado(dadosBrutos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Turmas.Should().HaveCount(1);
            var turma = resultado.Turmas.First();
            
            turma.AlunosAprovadosMunicipal?.Alunos.Should().BeEmpty();
            turma.AlunosAprovadosParceira?.Alunos.Should().BeEmpty();
            turma.AlunosReprovadosMunicipal?.Alunos.Should().BeEmpty();
            turma.AlunosReprovadosParceira?.Alunos.Should().BeEmpty();
            turma.RegentesDaTurma.Should().BeEmpty();
            turma.Cabecalho.DataDasAulasSincronas.Should().BeEmpty();
            turma.Cabecalho.Modalidade.Should().Be(ModalidadeRelatorioCodaf.Presencial);
        }

        [Theory]
        [InlineData("08:30", 8)]
        [InlineData("12", 0)] // Inválido (sem dois pontos)
        [InlineData("", 0)] // Vazio
        [InlineData("abc:def", 0)] // Texto
        public void DadoHorasVariadas_QuandoMapear_DeveConverterHoraCorretamente(string horaMinuto, int horaEsperada)
        {
            // Arrange
            var dadosBrutos = new DadosPrincipaisRelatorioCodafDto
            {
                CargaHorariaPresencial = horaMinuto,
                Participantes = [],
                RegentesTurma = [],
                DataAulas = []
            };

            // Act
            var resultado = RelatorioCodafDto.MapearParaDtoEstruturado(dadosBrutos);

            // Assert
            resultado.Turmas.First().Cabecalho.CargaHorariaPresencial.Should().Be(horaEsperada);
        }
    }
}
