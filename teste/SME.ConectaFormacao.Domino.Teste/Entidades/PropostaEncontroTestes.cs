using FluentAssertions;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class PropostaEncontroTestes
    {
        [Fact]
        public void DadoHoraInicioDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var encontro1 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            var encontro2 = new PropostaEncontro
            {
                HoraInicio = "09:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };

            // Act
            var houveAlteracao = encontro1.HouveAlteracao(encontro2);

            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoHoraFimDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var encontro1 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            var encontro2 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "13:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };

            // Act
            var houveAlteracao = encontro1.HouveAlteracao(encontro2);

            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoTipoDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var encontro1 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            var encontro2 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Assincrono,
                Local = "Sala 1"
            };

            // Act
            var houveAlteracao = encontro1.HouveAlteracao(encontro2);

            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoLocalDiferente_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarTrue()
        {
            // Arrange
            var encontro1 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            var encontro2 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 2"
            };
            // Act
            var houveAlteracao = encontro1.HouveAlteracao(encontro2);
            // Assert
            houveAlteracao.Should().BeTrue();
        }

        [Fact]
        public void DadoEntidadesIguais_QuandoCompararComOutraEntidade_EntaoHouveAlteracaoDeveRetornarFalse()
        {
            // Arrange
            var encontro1 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            var encontro2 = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            // Act
            var houveAlteracao = encontro1.HouveAlteracao(encontro2);
            // Assert
            houveAlteracao.Should().BeFalse();
        }

        [Fact]
        public void DadoEncontroComHorarioLegado_QuandoVerificarPossuiHorarioLegado_EntaoDeveRetornarTrue()
        {
            // Arrange
            var encontro = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };

            // Act
            var possuiHorarioLegado = encontro.PossuiHorarioLegado;

            // Assert
            possuiHorarioLegado.Should().BeTrue();
        }

        [Fact]
        public void DadoEncontroSemHorarioLegado_QuandoVerificarPossuiHorarioLegado_EntaoDeveRetornarFalse()
        {
            // Arrange
            var encontro = new PropostaEncontro
            {
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            // Act
            var possuiHorarioLegado = encontro.PossuiHorarioLegado;
            // Assert
            possuiHorarioLegado.Should().BeFalse();
        }

        [Fact]
        public void DadoEncontroComHorarioLegado_QuandoMigrarHorariosLegadoParaDatas_EntaoHorariosDevemSerMigradosParaDatas()
        {
            // Arrange
            var encontro = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1",
                Datas =
                [
                    new PropostaEncontroData { DataInicio = new (2023, 11, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new PropostaEncontroData { DataInicio = new (2023, 11, 2, 0, 0, 0, DateTimeKind.Utc) }
                ]
            };
            // Act
            encontro.MigrarHorariosLegadoParaDatas();
            // Assert
            foreach (var data in encontro.Datas)
            {
                data.HoraInicio.Should().Be("08:00");
                data.HoraFim.Should().Be("12:00");
            }
            encontro.HoraInicio.Should().BeNull();
            encontro.HoraFim.Should().BeNull();
        }

        [Fact]
        public void DadoEncontroComHorarioLegadoEDataComHorarioPreenchido_QuandoMigrarHorariosLegadoParaDatas_EntaoApenasDatasSemHorarioDevemSerPreenchidas()
        {
            // Arrange
            var encontro = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1",
                Datas =
                [
                    new PropostaEncontroData { DataInicio = new (2023, 11, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new PropostaEncontroData { DataInicio = new (2023, 11, 2, 0, 0, 0, DateTimeKind.Utc), HoraInicio = "09:00", HoraFim = "13:00" }
                ]
            };
            // Act
            encontro.MigrarHorariosLegadoParaDatas();
            // Assert
            encontro.Datas.First().HoraInicio.Should().Be("08:00");
            encontro.Datas.First().HoraFim.Should().Be("12:00");
            encontro.Datas.Last().HoraInicio.Should().Be("09:00");
            encontro.Datas.Last().HoraFim.Should().Be("13:00");
            encontro.HoraInicio.Should().BeNull();
            encontro.HoraFim.Should().BeNull();
        }

        [Fact]
        public void DadoEncontroSemHorarioLegado_QuandoMigrarHorariosLegadoParaDatas_EntaoNaoDeveAlterarDatas()
        {
            // Arrange
            var encontro = new PropostaEncontro
            {
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1",
                Datas =
                [
                    new PropostaEncontroData { DataInicio = new (2023, 11, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new PropostaEncontroData { DataInicio = new (2023, 11, 2, 0, 0, 0, DateTimeKind.Utc) }
                ]
            };
            // Act
            encontro.MigrarHorariosLegadoParaDatas();
            // Assert
            foreach (var data in encontro.Datas)
            {
                data.HoraInicio.Should().BeNull();
                data.HoraFim.Should().BeNull();
            }
        }

        [Fact]
        public void DadoEncontroSemDatasEComHorarioLegado_QuandoMigrarHorariosLegadoParaDatas_EntaoDeveLimparHorarioLegadoSemPreencherDatas()
        {
            // Arrange
            var encontro = new PropostaEncontro
            {
                HoraInicio = "08:00",
                HoraFim = "12:00",
                Tipo = TipoEncontro.Presencial,
                Local = "Sala 1"
            };
            // Act
            encontro.MigrarHorariosLegadoParaDatas();
            // Assert
            encontro.HoraInicio.Should().BeNull();
            encontro.HoraFim.Should().BeNull();
        }
    }
}