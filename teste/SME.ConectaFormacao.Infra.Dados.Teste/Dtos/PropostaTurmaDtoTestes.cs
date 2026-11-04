using FluentAssertions;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using System;
using Xunit;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Dtos
{
    public class PropostaTurmaDtoTestes
    {
        [Fact]
        public void Descricao_QuandoSetadaManualmente_DeveRetornarValorSetado()
        {
            // Arrange
            var dto = new PropostaTurmaDto
            {
                Id = 1,
                Nome = "Turma 1",
                Descricao = "Turma 1 Customizada"
            };

            // Assert
            dto.Descricao.Should().Be("Turma 1 Customizada");
        }

        [Fact]
        public void Descricao_QuandoNaoSetadaComDataInicioEDataFim_DeveRetornarIntervaloDeDatas()
        {
            // Arrange
            var dto = new PropostaTurmaDto
            {
                Id = 1,
                Nome = "Turma 1",
                DataInicio = new DateTime(2026, 5, 10),
                DataFim = new DateTime(2026, 6, 15)
            };

            // Assert
            dto.Descricao.Should().Be("Turma 1 10/05/2026 até 15/06/2026");
        }

        [Fact]
        public void Descricao_QuandoNaoSetadaComApenasDataInicio_DeveRetornarApenasDataInicio()
        {
            // Arrange
            var dto = new PropostaTurmaDto
            {
                Id = 1,
                Nome = "Turma 1",
                DataInicio = new DateTime(2026, 5, 10),
                DataFim = null
            };

            // Assert
            dto.Descricao.Should().Be("Turma 1 10/05/2026");
        }

        [Fact]
        public void Descricao_QuandoNaoSetadaESemDatas_DeveRetornarNome()
        {
            // Arrange
            var dto = new PropostaTurmaDto
            {
                Id = 1,
                Nome = "Turma 1",
                DataInicio = null,
                DataFim = null
            };

            // Assert
            dto.Descricao.Should().Be("Turma 1");
        }
    }
}
