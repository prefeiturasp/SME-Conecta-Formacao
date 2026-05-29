using FluentValidation;
using FluentValidation.TestHelper;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios.AtivarUsuarioExterno
{
    public class AtivarUsuarioExternoCommandTestes
    {
        private readonly AtivarUsuarioExternoCommandValidator _validator;

        public AtivarUsuarioExternoCommandTestes()
        {
            _validator = new AtivarUsuarioExternoCommandValidator();
        }

        #region Command - Construtor e Propriedades

        [Fact(DisplayName = "Command - Deve criar comando com login válido")]
        public void Deve_Criar_Comando_Com_Login_Valido()
        {
            // Arrange & Act
            var loginEsperado = "usuario.externo@teste.com";
            var command = new AtivarUsuarioExternoCommand(loginEsperado);

            // Assert
            Assert.NotNull(command);
            Assert.Equal(loginEsperado, command.Login);
        }

        [Fact(DisplayName = "Command - Deve criar comando com login vazio")]
        public void Deve_Criar_Comando_Com_Login_Vazio()
        {
            // Arrange & Act
            var command = new AtivarUsuarioExternoCommand(string.Empty);

            // Assert
            Assert.NotNull(command);
            Assert.Equal(string.Empty, command.Login);
        }

        [Fact(DisplayName = "Command - Propriedade Login deve ser readonly")]
        public void Propriedade_Login_Deve_Ser_Readonly()
        {
            // Arrange & Act
            var propertyInfo = typeof(AtivarUsuarioExternoCommand).GetProperty("Login");

            // Assert
            Assert.NotNull(propertyInfo);
            Assert.False(propertyInfo.CanWrite);
        }

        [Fact(DisplayName = "Command - Deve implementar IRequest de bool")]
        public void Deve_Implementar_IRequest_De_Bool()
        {
            // Arrange & Act
            var command = new AtivarUsuarioExternoCommand("usuario");

            // Assert
            Assert.IsType<MediatR.IRequest<bool>>(command, exactMatch: false);
        }

        #endregion

        #region Validator - Validações

        [Fact(DisplayName = "Validator - Deve retornar erro quando Login é null")]
        public void Validator_Deve_Retornar_Erro_Quando_Login_Null()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand(null!);

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldHaveValidationErrorFor(x => x.Login)
                .WithErrorMessage("É necessário informar o login do usuário externo para ativá-lo");
        }

        [Fact(DisplayName = "Validator - Deve retornar erro quando Login é vazio")]
        public void Validator_Deve_Passar_Quando_Login_Vazio()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand(string.Empty);

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve retornar erro quando Login é apenas espaços")]
        public void Validator_Deve_Passar_Quando_Login_Apenas_Espacos()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("   ");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar quando Login é válido")]
        public void Validator_Deve_Passar_Quando_Login_Valido()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("usuario.externo");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar quando Login é um email válido")]
        public void Validator_Deve_Passar_Quando_Login_Email_Valido()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("usuario@exemplo.com.br");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar quando Login é um login simples")]
        public void Validator_Deve_Passar_Quando_Login_Simples()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("usuario123");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar quando Login tem caracteres especiais permitidos")]
        public void Validator_Deve_Passar_Login_Caracteres_Especiais_Permitidos()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("usuario-123_externo.novo");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar quando Login tem tamanho máximo")]
        public void Validator_Deve_Passar_Login_Tamanho_Maximo()
        {
            // Arrange
            var loginGrande = new string('a', 255);
            var command = new AtivarUsuarioExternoCommand(loginGrande);

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar quando Login tem um caractere")]
        public void Validator_Deve_Passar_Login_Um_Caractere()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("a");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        #endregion

        #region Validator - Casos Extremos

        [Fact(DisplayName = "Validator - Não deve validar outras propriedades além do Login")]
        public void Validator_Nao_Deve_Validar_Outras_Propriedades()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand("usuario.valido");

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            Assert.DoesNotContain(resultado.Errors, e => !e.PropertyName.Equals("Login", StringComparison.Ordinal));
        }

        [Fact(DisplayName = "Validator - Deve usar apenas a regra NotNull para Login")]
        public void Validator_Deve_Usar_Apenas_NotNull_Para_Login()
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand(null!);

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            var erros = resultado.Errors.Where(e => e.PropertyName == "Login").ToList();
            Assert.Single(erros);
            Assert.Equal("É necessário informar o login do usuário externo para ativá-lo", erros.First().ErrorMessage);
        }

        #endregion

        #region Integração de Validação

        [Theory(DisplayName = "Validator - Teoria: Deve rejeitar logins inválidos")]
        [InlineData(null)]
        public void Validator_Teoria_Deve_Rejeitar_Logins_Invalidos(string? loginInvalido)
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand(loginInvalido!);

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            Assert.Contains(resultado.Errors, e => e.PropertyName == "Login");
        }

        [Theory(DisplayName = "Validator - Teoria: Deve aceitar logins válidos")]
        [InlineData("usuario")]
        [InlineData("usuario@teste.com")]
        [InlineData("usuario.externo")]
        [InlineData("USUARIO_123")]
        [InlineData("a")]
        public void Validator_Teoria_Deve_Aceitar_Logins_Validos(string loginValido)
        {
            // Arrange
            var command = new AtivarUsuarioExternoCommand(loginValido);

            // Act
            var resultado = _validator.TestValidate(command);

            // Assert
            Assert.DoesNotContain(resultado.Errors, e => e.PropertyName == "Login");
        }

        #endregion

        #region Instâncias de Validator

        [Fact(DisplayName = "Validator - Deve criar instância padrão")]
        public void Validator_Deve_Criar_Instancia_Padrao()
        {
            // Arrange & Act
            var validator = new AtivarUsuarioExternoCommandValidator();

            // Assert
            Assert.NotNull(validator);
            Assert.IsType<AbstractValidator<AtivarUsuarioExternoCommand>>(validator, exactMatch: false);
        }

        [Fact(DisplayName = "Validator - Diferentes instâncias devem ter mesmo comportamento")]
        public void Validator_Diferentes_Instancias_Mesmo_Comportamento()
        {
            // Arrange
            var validator1 = new AtivarUsuarioExternoCommandValidator();
            var validator2 = new AtivarUsuarioExternoCommandValidator();
            var command = new AtivarUsuarioExternoCommand(null!);

            // Act
            var resultado1 = validator1.TestValidate(command);
            var resultado2 = validator2.TestValidate(command);

            // Assert
            Assert.Equal(resultado1.Errors.Count, resultado2.Errors.Count);
            Assert.Equal(resultado1.Errors.First().ErrorMessage, resultado2.Errors.First().ErrorMessage);
        }

        #endregion
    }
}
