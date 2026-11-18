using SME.ConectaFormacao.Aplicacao.Dtos;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Dtos
{
    public class PropostaTurmaInscricaoDTOTestes
    {
        [Fact]
        public void DadoValoresValidos_QuandoAtribuirPropriedades_EntaoDeveArmazenarCorretamente()
        {
            // Arrange
            var dto = new PropostaTurmaInscricaoDTO
            {
                Id = 123,
                Descricao = "Turma 1 - 01/01 a 10/01",
                PossuiCodafListaPresenca = true
            };

            // Assert
            Assert.Equal(123, dto.Id);
            Assert.Equal("Turma 1 - 01/01 a 10/01", dto.Descricao);
            Assert.True(dto.PossuiCodafListaPresenca);
        }

        [Fact]
        public void DadoValoresPadrao_QuandoInstanciar_EntaoPossuiCodafListaPresencaDeveSerFalso()
        {
            // Arrange & Act
            var dto = new PropostaTurmaInscricaoDTO();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Null(dto.Descricao);
            Assert.False(dto.PossuiCodafListaPresenca);
        }
    }
}
