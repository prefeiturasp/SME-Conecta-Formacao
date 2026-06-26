using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Logs
{
    public class SalvarLogCommandTestes
    {
        [Fact]
        public void Deve_atribuir_LogDTO_no_construtor()
        {
            var logDto = new LogDTO
            {
                Id = 1,
                CriadoPor = "Usuário Teste",
                CriadoLogin = "usuario.teste"
            };

            var command = new SalvarLogCommand(logDto);

            Assert.Same(logDto, command.LogDTO);
        }
    }
}
