using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterPropostasDashboardTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterPropostasDashboard _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostasDashboardTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterPropostasDashboard>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioLogadoComAreaPromotora_QuandoExecutar_EntaoDeveEnviarQueryComAreaERetornarLista()
        {
            // Arrange
            var areaPromotoraId = _faker.Random.Long(1, 1000);
            var areaPromotora = new AreaPromotora
            {
                Id = areaPromotoraId,
                Nome = _faker.Company.CompanyName()
            };

            var filtro = new PropostaFiltrosDashboardDTO
            {
                NomeFormacao = _faker.Company.CatchPhrase()
            };

            var listaEsperada = new List<PropostaDashboardDTO>
            {
                new()
                {
                    TotalRegistros = "5",
                    Cor = "#FFFFFF"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterDashBoardQuery>(q =>
                        q.PropostaFiltrosDashboardDTO == filtro &&
                        q.AreaPromotoraIdUsuarioLogado == areaPromotoraId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaEsperada);

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(listaEsperada);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterDashBoardQuery>(q =>
                    q.PropostaFiltrosDashboardDTO == filtro &&
                    q.AreaPromotoraIdUsuarioLogado == areaPromotoraId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioLogadoSemAreaPromotora_QuandoExecutar_EntaoDeveEnviarQueryComAreaNulaERetornarLista()
        {
            // Arrange
            var filtro = new PropostaFiltrosDashboardDTO
            {
                NomeFormacao = _faker.Company.CatchPhrase()
            };

            var listaEsperada = new List<PropostaDashboardDTO>
            {
                new()
                {
                    TotalRegistros = "0"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaPromotora?)null);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterDashBoardQuery>(q =>
                        q.PropostaFiltrosDashboardDTO == filtro &&
                        q.AreaPromotoraIdUsuarioLogado == null),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaEsperada);

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(listaEsperada);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterDashBoardQuery>(q =>
                    q.PropostaFiltrosDashboardDTO == filtro &&
                    q.AreaPromotoraIdUsuarioLogado == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoObterAreaPromotora_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var filtro = new PropostaFiltrosDashboardDTO();
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(filtro);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterDashBoardQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroAoObterDashboard_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var filtro = new PropostaFiltrosDashboardDTO();
            var areaPromotora = new AreaPromotora
            {
                Id = _faker.Random.Long(1, 1000)
            };
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDashBoardQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(filtro);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterDashBoardQuery>(q =>
                    q.PropostaFiltrosDashboardDTO == filtro &&
                    q.AreaPromotoraIdUsuarioLogado == areaPromotora.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
