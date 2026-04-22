using FluentAssertions;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class PropostaEncontroDataTestes
    {
        [Fact]
        public void DadoDataInicioDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var data1 = new PropostaEncontroData
            {
                PropostaEncontroId = 1,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };
            var data2 = new PropostaEncontroData
            {
                PropostaEncontroId = data1.PropostaEncontroId,
                DataInicio = new(2024, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };

            // Act
            var houveAlteracao = data1.HouveAlteracao(data2);

            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoDataFimDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var data1 = new PropostaEncontroData
            {
                PropostaEncontroId = 1,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };
            var data2 = new PropostaEncontroData
            {
                PropostaEncontroId = data1.PropostaEncontroId,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };
            // Act
            var houveAlteracao = data1.HouveAlteracao(data2);
            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoHoraInicioDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var data1 = new PropostaEncontroData
            {
                PropostaEncontroId = 1,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };
            var data2 = new PropostaEncontroData
            {
                PropostaEncontroId = data1.PropostaEncontroId,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "09:00",
                HoraFim = "12:00"
            };
            // Act
            var houveAlteracao = data1.HouveAlteracao(data2);
            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoHoraFimDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var data1 = new PropostaEncontroData
            {
                PropostaEncontroId = 1,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };
            var data2 = new PropostaEncontroData
            {
                PropostaEncontroId = data1.PropostaEncontroId,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "13:00"
            };
            // Act
            var houveAlteracao = data1.HouveAlteracao(data2);
            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoOutraEntidadeComMesmosDados_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarFalse()
        {
            // Arrange
            var data1 = new PropostaEncontroData
            {
                PropostaEncontroId = 1,
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                HoraInicio = "08:00",
                HoraFim = "12:00"
            };
            var data2 = new PropostaEncontroData
            {
                PropostaEncontroId = data1.PropostaEncontroId,
                DataInicio = data1.DataInicio,
                DataFim = data1.DataFim,
                HoraInicio = data1.HoraInicio,
                HoraFim = data1.HoraFim
            };
            // Act
            var houveAlteracao = data1.HouveAlteracao(data2);
            // Assert
            houveAlteracao.Should().BeFalse();
        }

        [Fact]
        public void DadoDataFimNula_QuandoVerificarSemDataFim_EntaoDeveRetornarTrue()
        {
            // Arrange
            var data = new PropostaEncontroData
            {
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            // Act
            var semDataFim = data.SemDataFim();
            // Assert
            semDataFim.Should().BeTrue();
        }

        [Fact]
        public void DadoDataInicioIgualDataFim_QuandoVerificarSemDataFim_EntaoDeveRetornarTrue()
        {
            // Arrange
            var data = new PropostaEncontroData
            {
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            // Act
            var semDataFim = data.SemDataFim();
            // Assert
            semDataFim.Should().BeTrue();
        }

        [Fact]
        public void DadoDataInicioDiferenteDataFim_QuandoVerificarSemDataFim_EntaoDeveRetornarFalse()
        {
            // Arrange
            var data = new PropostaEncontroData
            {
                DataInicio = new(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DataFim = new(2024, 10, 2, 0, 0, 0, DateTimeKind.Utc)
            };
            // Act
            var semDataFim = data.SemDataFim();
            // Assert
            semDataFim.Should().BeFalse();
        }
    }
}