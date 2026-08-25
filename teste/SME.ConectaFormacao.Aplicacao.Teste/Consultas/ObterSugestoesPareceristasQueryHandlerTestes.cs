using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    [ExcludeFromCodeCoverage]
    public class ObterSugestoesPareceristasQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterSugestoesPareceristasQueryHandler _sut;

        public ObterSugestoesPareceristasQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterSugestoesPareceristasQueryHandler>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoExecutar_EntaoRetornaSugestoesPareceristas()
        {
            // Arrange
            var query = new ObterSugestoesPareceristasQuery(1);

            var pareceristas = new List<PropostaParecerista>
            {
                new PropostaParecerista { Id = 1 }
            };

            var dtos = new List<PropostaPareceristaSugestaoDTO>
            {
                new PropostaPareceristaSugestaoDTO { }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterSugestaoParecerPareceristas(1))
                .ReturnsAsync(pareceristas);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<IEnumerable<PropostaPareceristaSugestaoDTO>>(pareceristas))
                .Returns(dtos);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(dtos);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.ObterSugestaoParecerPareceristas(1), Times.Once);
        }
    }
}
