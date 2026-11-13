using FluentValidation.TestHelper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Enumerados;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Dtos
{
    public class CodafAnexoSalvarValidatorTestes
    {
        private readonly CodafAnexoSalvarValidator _validator;

        public CodafAnexoSalvarValidatorTestes()
        {
            _validator = new CodafAnexoSalvarValidator();
        }

        [Fact]
        public void DadoArquivoCodigoVazio_QuandoValidar_EntaoDeveRetornarErro()
        {
            // Arrange
            var dto = new CodafAnexoSalvarDto
            {
                ArquivoCodigo = Guid.Empty,
                NomeArquivo = "arquivo.pdf",
                TipoAnexoId = TipoAnexoCodaf.ListaPresenca
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(c => c.ArquivoCodigo)
                .WithErrorMessage("O código do arquivo é obrigatório.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DadoNomeArquivoVazioOuNulo_QuandoValidar_EntaoDeveRetornarErro(string? nomeArquivo)
        {
            // Arrange
            var dto = new CodafAnexoSalvarDto
            {
                ArquivoCodigo = Guid.NewGuid(),
                NomeArquivo = nomeArquivo!,
                TipoAnexoId = TipoAnexoCodaf.ListaPresenca
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(c => c.NomeArquivo)
                .WithErrorMessage("O nome do arquivo é obrigatório.");
        }

        [Fact]
        public void DadoTipoAnexoInvalido_QuandoValidar_EntaoDeveRetornarErro()
        {
            // Arrange
            var dto = new CodafAnexoSalvarDto
            {
                ArquivoCodigo = Guid.NewGuid(),
                NomeArquivo = "arquivo.pdf",
                TipoAnexoId = (TipoAnexoCodaf)0
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor(c => c.TipoAnexoId)
                .WithErrorMessage("Tipo de anexo inválido.");
        }

        [Fact]
        public void DadoDadosValidos_QuandoValidar_EntaoNaoDeveRetornarErro()
        {
            // Arrange
            var dto = new CodafAnexoSalvarDto
            {
                ArquivoCodigo = Guid.NewGuid(),
                NomeArquivo = "arquivo.pdf",
                TipoAnexoId = TipoAnexoCodaf.ListaPresenca
            };

            // Act
            var resultado = _validator.TestValidate(dto);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void DadoListaPresencaCadastroComAnexoComGuidZerado_QuandoValidar_EntaoDeveRetornarErro()
        {
            // Arrange
            var validator = new CodafListaPresencaCadastroValidator();
            var dto = new CodafListaPresencaCadastroDto
            {
                Anexos = new List<CodafAnexoSalvarDto>
                {
                    new()
                    {
                        ArquivoCodigo = Guid.Empty,
                        NomeArquivo = "lista.pdf",
                        TipoAnexoId = TipoAnexoCodaf.ListaPresenca
                    }
                }
            };

            // Act
            var resultado = validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor("Anexos[0].ArquivoCodigo")
                .WithErrorMessage("O código do arquivo é obrigatório.");
        }

        [Fact]
        public void DadoListaPresencaEdicaoComAnexoComGuidZerado_QuandoValidar_EntaoDeveRetornarErro()
        {
            // Arrange
            var validator = new CodafListaPresencaEdicaoValidator();
            var dto = new CodafListaPresencaEdicaoDto
            {
                Anexos = new List<CodafAnexoSalvarDto>
                {
                    new()
                    {
                        ArquivoCodigo = Guid.Empty,
                        NomeArquivo = "lista.pdf",
                        TipoAnexoId = TipoAnexoCodaf.ListaPresenca
                    }
                }
            };

            // Act
            var resultado = validator.TestValidate(dto);

            // Assert
            resultado.ShouldHaveValidationErrorFor("Anexos[0].ArquivoCodigo")
                .WithErrorMessage("O código do arquivo é obrigatório.");
        }
    }
}
