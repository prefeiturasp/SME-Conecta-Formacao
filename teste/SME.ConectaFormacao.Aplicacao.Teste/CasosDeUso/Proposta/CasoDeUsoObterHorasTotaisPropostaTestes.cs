using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Extensoes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterHorasTotaisPropostaTestes
    {
        private readonly CasoDeUsoObterHorasTotaisProposta _casoDeUso;

        public CasoDeUsoObterHorasTotaisPropostaTestes()
        {
            var mocker = new AutoMocker();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterHorasTotaisProposta>();
        }

        [Fact]
        public async Task DadoExecucao_QuandoChamarExecutar_EntaoDeveRetornarHorasTotaisProposta()
        {
            // Arrange
            var horasTotaisEsperadas = Enum.GetValues(typeof(HorasTotaisProposta))
                .Cast<HorasTotaisProposta>()
                .Select(t => new RetornoListagemDTO
                {
                    Id = (short)t,
                    Descricao = t.Nome()
                });

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(horasTotaisEsperadas);
        }
    }
}
