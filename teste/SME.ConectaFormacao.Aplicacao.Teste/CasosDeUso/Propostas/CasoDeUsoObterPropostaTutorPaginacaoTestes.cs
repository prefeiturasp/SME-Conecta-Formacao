using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterPropostaTutorPaginacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterPropostaTutorPaginacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaTutorPaginacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = mocker.CreateInstance<CasoDeUsoObterPropostaTutorPaginacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoIdIgualAZero_QuandoExecutar_EntaoDeveRetornarPaginacaoVaziaSemChamarMediator()
        {
            // Arrange
            const long id = 0;

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Items.Should().BeEmpty();
            resultado.TotalRegistros.Should().Be(0);
            resultado.TotalPaginas.Should().Be(0);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterTutorPaginadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoIdValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarResultadoPaginado()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var listaTutores = new List<PropostaTutorDTO>
            {
                new()
                {
                    Id = 1,
                    NomeTutor = _faker.Person.FullName,
                    Cpf = _faker.Random.Replace("###.###.###-##")
                }
            };
            var resultadoEsperado = new PaginacaoResultadoDto<PropostaTutorDTO>(
                items: listaTutores,
                totalRegistros: 1,
                numeroRegistros: 10
            );

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterTutorPaginadoQuery>(q =>
                        q.PropostaId == id &&
                        q.NumeroPagina == _sut.NumeroPagina &&
                        q.NumeroRegistros == _sut.NumeroRegistros),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(resultadoEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterTutorPaginadoQuery>(q =>
                    q.PropostaId == id &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoCustomizada_QuandoExecutar_EntaoDeveRepassarValoresCustomizadosNaQuery()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            _sut.NumeroPagina = 3;
            _sut.NumeroRegistros = 25;

            var resultadoEsperado = new PaginacaoResultadoDto<PropostaTutorDTO>(
                items: new List<PropostaTutorDTO>(),
                totalRegistros: 0,
                numeroRegistros: 25
            );

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterTutorPaginadoQuery>(q =>
                        q.PropostaId == id &&
                        q.NumeroPagina == 3 &&
                        q.NumeroRegistros == 25),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(resultadoEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterTutorPaginadoQuery>(q =>
                    q.PropostaId == id &&
                    q.NumeroPagina == 3 &&
                    q.NumeroRegistros == 25),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterTutorPaginadoQuery>(q => q.PropostaId == id),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterTutorPaginadoQuery>(q =>
                    q.PropostaId == id &&
                    q.NumeroPagina == _sut.NumeroPagina &&
                    q.NumeroRegistros == _sut.NumeroRegistros),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
