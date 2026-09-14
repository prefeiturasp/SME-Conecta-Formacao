using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos;
using EntidadeAreaPromotora = SME.ConectaFormacao.Dominio.Entidades.AreaPromotora;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAreaPromotoraListaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterAreaPromotoraLista _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterAreaPromotoraListaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterAreaPromotoraLista>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioLogadoComArea_QuandoExecutar_EntaoDeveConsultarComAreaIdERetornarLista()
        {
            // Arrange
            var areaId = _faker.Random.Long(1, 100);
            var areaUsuario = new EntidadeAreaPromotora { Id = areaId };
            var listaEsperada = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Área 1" },
                new() { Id = 2, Descricao = "Área 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), default))
                .ReturnsAsync(areaUsuario);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == areaId), default))
                .ReturnsAsync(listaEsperada);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(listaEsperada);

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == areaId), default), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioLogadoSemArea_QuandoExecutar_EntaoDeveConsultarComAreaIdNuloERetornarLista()
        {
            // Arrange
            var listaEsperada = new List<RetornoListagemDTO>
            {
                new() { Id = 3, Descricao = "Área Geral" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), default))
                .ReturnsAsync((EntidadeAreaPromotora?)null);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == null), default))
                .ReturnsAsync(listaEsperada);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(listaEsperada);

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterAreaPromotoraListaQuery>(q => q.AreaPromotoraIdUsuarioLogado == null), default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
