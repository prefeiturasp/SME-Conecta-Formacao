using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class AlterarNomeSocialServicoAcessosQueryTestes
    {
        private const string MensagemLoginObrigatorio =
       "É necessário informar o login para alterar o nome social do usuário";

        [Fact]
        public void Command_Deve_armazenar_login_e_nome_social()
        {
            var command = new AlterarNomeSocialServicoAcessosCommand(
                "52998224725",
                "Maria da Silva");

            Assert.Equal("52998224725", command.Login);
            Assert.Equal("Maria da Silva", command.NomeSocial);
        }

        [Fact]
        public void Command_Deve_permitir_nome_social_nulo()
        {
            var command = new AlterarNomeSocialServicoAcessosCommand(
                "52998224725",
                null);

            Assert.Equal("52998224725", command.Login);
            Assert.Null(command.NomeSocial);
        }

        [Theory]
        [InlineData("Maria da Silva")]
        [InlineData(null)]
        [InlineData("")]
        public void Validator_Quando_login_for_valido_Deve_aceitar_qualquer_nome_social(
            string? nomeSocial)
        {
            var validator = new AlterarNomeSocialServicoAcessosCommandValidator();
            var command = new AlterarNomeSocialServicoAcessosCommand(
                "52998224725",
                nomeSocial);

            var resultado = validator.Validate(command);

            Assert.True(resultado.IsValid);
            Assert.Empty(resultado.Errors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validator_Quando_login_nao_for_informado_Deve_retornar_erro(
            string? login)
        {
            var validator = new AlterarNomeSocialServicoAcessosCommandValidator();
            var command = new AlterarNomeSocialServicoAcessosCommand(
                login!,
                "Maria da Silva");

            var resultado = validator.Validate(command);

            Assert.False(resultado.IsValid);
            var erro = Assert.Single(resultado.Errors);
            Assert.Equal(nameof(AlterarNomeSocialServicoAcessosCommand.Login), erro.PropertyName);
            Assert.Equal(MensagemLoginObrigatorio, erro.ErrorMessage);
        }
    }
}
