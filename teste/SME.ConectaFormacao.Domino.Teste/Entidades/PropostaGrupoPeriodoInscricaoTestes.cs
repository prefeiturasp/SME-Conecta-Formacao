using FluentAssertions;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class PropostaGrupoPeriodoInscricaoTestes
    {
        [Fact]
        public void DadoNovaTurma_QuandoAdicionarTurma_EntaoDeveAdicionarNaLista()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao { Id = 10, PropostaId = 1 };

            // Act
            grupo.AdicionarTurma(100);

            // Assert
            grupo.TurmasVinculadas.Should().HaveCount(1);
            var turma = grupo.TurmasVinculadas.First();
            turma.GrupoPeriodoInscricaoId.Should().Be(10);
            turma.PropostaTurmaId.Should().Be(100);
            turma.Excluido.Should().BeFalse();
        }

        [Fact]
        public void DadoTurmaPreviamenteExcluida_QuandoAdicionarMesmaTurma_EntaoDeveReativarVinculo()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao { Id = 10 };
            grupo.AdicionarTurma(100);
            grupo.TurmasVinculadas.First().Excluido = true;

            // Act
            grupo.AdicionarTurma(100);

            // Assert
            grupo.TurmasVinculadas.Should().HaveCount(1);
            grupo.TurmasVinculadas.First().Excluido.Should().BeFalse();
        }

        [Fact]
        public void DadoTurmaJaAtiva_QuandoAdicionarNovamente_EntaoNaoDeveDuplicar()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao { Id = 10 };
            grupo.AdicionarTurma(100);

            // Act
            grupo.AdicionarTurma(100);

            // Assert
            grupo.TurmasVinculadas.Should().HaveCount(1);
            grupo.TurmasVinculadas.First().Excluido.Should().BeFalse();
        }

        [Fact]
        public void DadoGrupoComTurmas_QuandoChamarExcluir_EntaoGrupoETurmasDevemSerExcluidos()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao { Id = 10 };
            grupo.AdicionarTurma(100);
            grupo.AdicionarTurma(200);

            // Act
            grupo.Excluir();

            // Assert
            grupo.Excluido.Should().BeTrue();
            grupo.TurmasVinculadas.Should().AllSatisfy(t => t.Excluido.Should().BeTrue());
        }

        [Fact]
        public void DadoListaDeTurmasDesejadas_QuandoSincronizarTurmas_EntaoDeveAtualizarVinculos()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao { Id = 10 };
            grupo.AdicionarTurma(100);
            grupo.AdicionarTurma(200);

            // Act: remove 100, mantém 200, adiciona 300
            grupo.SincronizarTurmas([200, 300]);

            // Assert
            var turma100 = grupo.TurmasVinculadas.FirstOrDefault(t => t.PropostaTurmaId == 100);
            var turma200 = grupo.TurmasVinculadas.FirstOrDefault(t => t.PropostaTurmaId == 200);
            var turma300 = grupo.TurmasVinculadas.FirstOrDefault(t => t.PropostaTurmaId == 300);

            turma100.Should().NotBeNull();
            turma100!.Excluido.Should().BeTrue();

            turma200.Should().NotBeNull();
            turma200!.Excluido.Should().BeFalse();

            turma300.Should().NotBeNull();
            turma300!.Excluido.Should().BeFalse();
        }

        [Fact]
        public void DadoListaNulaDeTurmas_QuandoSincronizarTurmas_EntaoDeveExcluirTodasTurmas()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao { Id = 10 };
            grupo.AdicionarTurma(100);

            // Act
            grupo.SincronizarTurmas(null!);

            // Assert
            grupo.TurmasVinculadas.First().Excluido.Should().BeTrue();
        }

        [Fact]
        public void DadoEntidadesCriadas_QuandoAtribuirPropriedades_EntaoDevemEstarCorretas()
        {
            // Arrange & Act
            var inicio = DateTime.Today;
            var fim = DateTime.Today.AddDays(5);
            var grupo = new PropostaGrupoPeriodoInscricao
            {
                Id = 1,
                PropostaId = 2,
                Descricao = "Turmas A e B",
                DataInicio = inicio,
                DataFim = fim
            };

            var vinculo = new PropostaGrupoPeriodoInscricaoTurma
            {
                GrupoPeriodoInscricaoId = 1,
                PropostaTurmaId = 10,
                GrupoPeriodoInscricao = grupo,
                PropostaTurma = new PropostaTurma { Id = 10, Nome = "Turma 10" }
            };

            // Assert
            grupo.Id.Should().Be(1);
            grupo.PropostaId.Should().Be(2);
            grupo.Descricao.Should().Be("Turmas A e B");
            grupo.DataInicio.Should().Be(inicio);
            grupo.DataFim.Should().Be(fim);

            vinculo.GrupoPeriodoInscricaoId.Should().Be(1);
            vinculo.PropostaTurmaId.Should().Be(10);
            vinculo.GrupoPeriodoInscricao.Should().Be(grupo);
            vinculo.PropostaTurma.Nome.Should().Be("Turma 10");
        }

        [Theory]
        [InlineData("2026-11-04 10:00:00", true)]
        [InlineData("2026-11-01 00:00:00", true)]
        [InlineData("2026-11-10 23:59:59", true)]
        [InlineData("2026-10-31 23:59:59", false)]
        [InlineData("2026-11-11 00:00:00", false)]
        public void DadoPeriodoComHoraZerada_QuandoVerificarEstaEmPeriodo_EntaoDeveConsiderarAteFinalDoDia(string dataRefStr, bool esperado)
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao
            {
                DataInicio = new DateTime(2026, 11, 1, 0, 0, 0),
                DataFim = new DateTime(2026, 11, 10, 0, 0, 0)
            };
            var dataRef = DateTime.Parse(dataRefStr);

            // Act
            var resultado = grupo.EstaEmPeriodo(dataRef);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Fact]
        public void DadoPeriodoComHoraEspecificada_QuandoVerificarEstaEmPeriodo_EntaoDeveRespeitarHoraExata()
        {
            // Arrange
            var grupo = new PropostaGrupoPeriodoInscricao
            {
                DataInicio = new DateTime(2026, 11, 1, 8, 0, 0),
                DataFim = new DateTime(2026, 11, 10, 18, 0, 0)
            };

            // Act & Assert
            grupo.EstaEmPeriodo(new DateTime(2026, 11, 10, 17, 59, 59)).Should().BeTrue();
            grupo.EstaEmPeriodo(new DateTime(2026, 11, 10, 18, 0, 1)).Should().BeFalse();
        }
    }
}
