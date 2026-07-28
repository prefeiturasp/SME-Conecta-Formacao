namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class SalvarUsuarioParcialCommandTestes
    {
        private const string MensagemLoginObrigatorio =
        "É necessário informar o login do usuário para alterar usuário";

        private const string MensagemNomeObrigatorio =
            "É necessário informar o nome do usuário para alterar usuário";

        [Fact]
        public void Command_Deve_armazenar_login_nome_e_nome_social()
        {
            var command = new SalvarUsuarioParcialCommand(
                "52998224725",
                "Maria da Silva")
            {
                NomeSocial = "Maria Souza"
            };

            Assert.Equal("52998224725", command.Login);
            Assert.Equal("Maria da Silva", command.Nome);
            Assert.Equal("Maria Souza", command.NomeSocial);
        }

        [Fact]
        public void Command_Deve_iniciar_nome_social_como_nulo()
        {
            var command = new SalvarUsuarioParcialCommand(
                "52998224725",
                "Maria da Silva");

            Assert.Null(command.NomeSocial);
        }

        [Fact]
        public void Validator_Quando_login_e_nome_forem_validos_Deve_ser_valido()
        {
            var validator = new SalvarUsuarioParcialCommandValidator();
            var command = new SalvarUsuarioParcialCommand(
                "52998224725",
                "Maria da Silva");

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
            var validator = new SalvarUsuarioParcialCommandValidator();
            var command = new SalvarUsuarioParcialCommand(
                login!,
                "Maria da Silva");

            var resultado = validator.Validate(command);

            Assert.False(resultado.IsValid);
            var erro = Assert.Single(resultado.Errors);
            Assert.Equal(nameof(SalvarUsuarioParcialCommand.Login), erro.PropertyName);
            Assert.Equal(MensagemLoginObrigatorio, erro.ErrorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validator_Quando_nome_nao_for_informado_Deve_retornar_erro(
            string? nome)
        {
            var validator = new SalvarUsuarioParcialCommandValidator();
            var command = new SalvarUsuarioParcialCommand(
                "52998224725",
                nome!);

            var resultado = validator.Validate(command);

            Assert.False(resultado.IsValid);
            var erro = Assert.Single(resultado.Errors);
            Assert.Equal(nameof(SalvarUsuarioParcialCommand.Nome), erro.PropertyName);
            Assert.Equal(MensagemNomeObrigatorio, erro.ErrorMessage);
        }

        [Fact]
        public void Validator_Quando_login_e_nome_nao_forem_informados_Deve_retornar_os_dois_erros()
        {
            var validator = new SalvarUsuarioParcialCommandValidator();
            var command = new SalvarUsuarioParcialCommand(string.Empty, string.Empty);

            var resultado = validator.Validate(command);

            Assert.False(resultado.IsValid);
            Assert.Equal(2, resultado.Errors.Count);

            Assert.Contains(resultado.Errors, erro =>
                erro.PropertyName == nameof(SalvarUsuarioParcialCommand.Login) &&
                erro.ErrorMessage == MensagemLoginObrigatorio);

            Assert.Contains(resultado.Errors, erro =>
                erro.PropertyName == nameof(SalvarUsuarioParcialCommand.Nome) &&
                erro.ErrorMessage == MensagemNomeObrigatorio);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Maria Souza")]
        public void Validator_Quando_dados_obrigatorios_forem_validos_Deve_ignorar_nome_social(
            string? nomeSocial)
        {
            var validator = new SalvarUsuarioParcialCommandValidator();
            var command = new SalvarUsuarioParcialCommand(
                "52998224725",
                "Maria da Silva")
            {
                NomeSocial = nomeSocial
            };

            var resultado = validator.Validate(command);

            Assert.True(resultado.IsValid);
            Assert.Empty(resultado.Errors);
        }
    }
}
