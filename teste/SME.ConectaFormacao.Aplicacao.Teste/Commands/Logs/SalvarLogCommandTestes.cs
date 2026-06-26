using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Logs
{
    public class SalvarLogCommandTestes
    {
        [Fact]
        public void Deve_atribuir_LogDTO_no_construtor()
        {
            var logDto = new LogDto
            {
                Id = 1,
                CriadoPor = "Usuário Teste",
                CriadoLogin = "usuario.teste"
            };

            var command = new SalvarLogCommand(logDto.Entidade, logDto.NivelLog, logDto.Mensagem, logDto.Complemento);

            Assert.Equal(logDto.Entidade, command.Entidade);
            Assert.Equal(logDto.NivelLog, command.NivelLog);
            Assert.Equal(logDto.Mensagem, command.Mensagem);
            Assert.Equal(logDto.Complemento, command.Complemento);
        }
    }
}
