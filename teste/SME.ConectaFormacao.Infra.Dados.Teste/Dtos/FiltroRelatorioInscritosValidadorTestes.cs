using FluentAssertions;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Dtos
{
    public class FiltroRelatorioInscritosValidadorTestes
    {
        [Fact]
        public void DadoFiltroNulo_QuandoValidar_EntaoDeveRetornarErroValidacao()
        {
            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(null!);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("Filtros para geração do relatório não informados");
        }

        [Fact]
        public void DadoCodigoFormacaoInformadoESemPeriodo_QuandoValidar_EntaoDeveRetornarSucesso()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 12345
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }

        [Fact]
        public void DadoCodigoFormacaoInformadoEPeriodoValido_QuandoValidar_EntaoDeveRetornarSucesso()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 12345,
                PeriodoDeRealizacaoInicial = new DateTime(2026, 1, 1),
                PeriodoDeRealizacaoFinal = new DateTime(2026, 6, 1)
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }

        [Fact]
        public void DadoCodigoFormacaoNaoInformadoESemPeriodo_QuandoValidar_EntaoDeveRetornarErroPeriodoObrigatorio()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = null
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("O período de realização deve ser informado");
        }

        [Fact]
        public void DadoCodigoFormacaoInvalidoMenorOuIgualAZeroESemPeriodo_QuandoValidar_EntaoDeveRetornarErroPeriodoObrigatorio()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 0
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("O período de realização deve ser informado");
        }

        [Fact]
        public void DadoCodigoFormacaoInformadoEApenasDataInicialInformada_QuandoValidar_EntaoDeveRetornarErroPeriodoObrigatorio()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 12345,
                PeriodoDeRealizacaoInicial = new DateTime(2026, 1, 1)
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("O período de realização deve ser informado");
        }

        [Fact]
        public void DadoCodigoFormacaoInformadoEApenasDataFinalInformada_QuandoValidar_EntaoDeveRetornarErroPeriodoObrigatorio()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 12345,
                PeriodoDeRealizacaoFinal = new DateTime(2026, 6, 1)
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("O período de realização deve ser informado");
        }

        [Fact]
        public void DadoPeriodoComDataFinalMenorQueDataInicial_QuandoValidar_EntaoDeveRetornarErroPeriodoFinalMenorQueInicial()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 12345,
                PeriodoDeRealizacaoInicial = new DateTime(2026, 6, 1),
                PeriodoDeRealizacaoFinal = new DateTime(2026, 1, 1)
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("O período de realização final não pode ser menor que o período de realização inicial");
        }

        [Fact]
        public void DadoPeriodoSuperiorAUmAno_QuandoValidar_EntaoDeveRetornarErroPeriodoSuperiorUmAno()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = 12345,
                PeriodoDeRealizacaoInicial = new DateTime(2026, 1, 1),
                PeriodoDeRealizacaoFinal = new DateTime(2027, 1, 2)
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("O período de realização não pode ser superior a 1 ano");
        }

        [Fact]
        public void DadoCodigoFormacaoNaoInformadoMasPeriodoValido_QuandoValidar_EntaoDeveRetornarSucesso()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = null,
                PeriodoDeRealizacaoInicial = new DateTime(2026, 1, 1),
                PeriodoDeRealizacaoFinal = new DateTime(2026, 12, 31)
            };

            // Act
            var resultado = FiltroRelatorioInscritosValidador.Validar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }
    }
}
