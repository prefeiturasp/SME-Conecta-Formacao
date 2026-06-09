using FluentAssertions;
using FluentValidation.TestHelper;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class SalvarUsuarioTelefoneParcialCommandTestes
    {
        [Fact]
        public void Deve_criar_command_com_dados_informados()
        {
            var login = "123456";
            var telefone = "11999999999";

            var command = new SalvarUsuarioTelefoneParcialCommand(login, telefone);

            command.Login.Should().Be(login);
            command.Telefone.Should().Be(telefone);
        }
    }

    public class SalvarUsuarioTelefoneParcialCommandValidatorTeste
    {
        private readonly SalvarUsuarioTelefoneParcialCommandValidator validator;
        public SalvarUsuarioTelefoneParcialCommandValidatorTeste()
        {
            validator = new SalvarUsuarioTelefoneParcialCommandValidator();
        }

        [Fact]
        public void Nao_deve_retornar_erro_quando_login_for_valido()
        {
            var command = new SalvarUsuarioTelefoneParcialCommand(
                "123456",
                "11999999999");

            var resultado = validator.TestValidate(command);

            resultado.ShouldNotHaveValidationErrorFor(x => x.Login);
        }

        [Fact]
        public void Deve_retornar_erro_quando_login_for_vazio()
        {
            var command = new SalvarUsuarioTelefoneParcialCommand(
                string.Empty,
                "11999999999");

            var resultado = validator.TestValidate(command);

            resultado.ShouldHaveValidationErrorFor(x => x.Login)
                .WithErrorMessage("É necessário informar o login do usuário para alterar usuário");
        }

        [Fact]
        public void Deve_retornar_erro_quando_login_for_nulo()
        {
            var command = new SalvarUsuarioTelefoneParcialCommand(
                null!,
                "11999999999");

            var resultado = validator.TestValidate(command);

            resultado.ShouldHaveValidationErrorFor(x => x.Login)
                .WithErrorMessage("É necessário informar o login do usuário para alterar usuário");
        }
    }
}
