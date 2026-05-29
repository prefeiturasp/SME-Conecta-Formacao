using FluentValidation.TestHelper;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Arquivo
{
    public class RemoverArquivoPorIdCommandTestes
    {
        private readonly RemoverArquivoPorIdCommandValidator _validator;

        public RemoverArquivoPorIdCommandTestes()
        {
            _validator = new RemoverArquivoPorIdCommandValidator();
        }

        [Fact(DisplayName = "Validator - Deve retornar erro quando Id for zero")]
        public void Deve_Retornar_Erro_Quando_Id_For_Zero()
        {
            var command = new RemoverArquivoPorIdCommand(0);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("É necessário informar o id do arquivo para remover");
        }

        [Fact(DisplayName = "Validator - Deve aceitar Id negativo")]
        public void Deve_Aceitar_Id_Negativo()
        {
            var command = new RemoverArquivoPorIdCommand(-1);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldNotHaveValidationErrorFor(x => x.Id);
        }

        [Fact(DisplayName = "Validator - Deve passar com Id válido")]
        public void Deve_Passar_Com_Id_Valido()
        {
            var command = new RemoverArquivoPorIdCommand(1);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Validator - Deve passar com Id maior que um")]
        public void Deve_Passar_Com_Id_Maior_Que_Um()
        {
            var command = new RemoverArquivoPorIdCommand(long.MaxValue);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Command - Deve criar comando com Id válido")]
        public void Deve_Criar_Comando_Com_Id_Valido()
        {
            var idEsperado = 123L;

            var command = new RemoverArquivoPorIdCommand(idEsperado);

            Assert.NotNull(command);
            Assert.Equal(idEsperado, command.Id);
        }

        [Fact(DisplayName = "Command - Deve implementar IRequest de bool")]
        public void Deve_Implementar_IRequest_De_Bool()
        {
            var command = new RemoverArquivoPorIdCommand(1);

            Assert.IsType<MediatR.IRequest<bool>>(command, exactMatch: false);
        }

        [Fact(DisplayName = "Command - Propriedade Id deve ser readonly")]
        public void Propriedade_Id_Deve_Ser_Readonly()
        {
            var propertyInfo = typeof(RemoverArquivoPorIdCommand).GetProperty("Id");
            Assert.NotNull(propertyInfo);
            Assert.False(propertyInfo.CanWrite);
        }
    }
}
