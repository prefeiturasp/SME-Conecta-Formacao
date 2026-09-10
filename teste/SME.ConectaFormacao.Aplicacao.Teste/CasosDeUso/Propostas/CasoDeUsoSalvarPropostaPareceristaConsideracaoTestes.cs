using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoSalvarPropostaPareceristaConsideracaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSalvarPropostaPareceristaConsideracao _sut;
        private readonly Faker _faker;

        public CasoDeUsoSalvarPropostaPareceristaConsideracaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoSalvarPropostaPareceristaConsideracao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoParametrosValidos_QuandoExecutar_EntaoDeveObterUsuarioLogadoEnviarCommandERetornarResultado()
        {
            // Arrange
            var usuarioLogado = new Usuario { Login = "1234567" };
            var dto = new PropostaPareceristaConsideracaoCadastroDTO
            {
                PropostaId = _faker.Random.Long(1, 100),
                Campo = CampoConsideracao.DescricaoDaAtividade,
                Descricao = _faker.Lorem.Sentence()
            };
            var retornoEsperado = new RetornoDTO { Sucesso = true };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarPropostaPareceristaConsideracaoCommand>(c =>
                    c.PropostaPareceristaConsideracaoCadastroDto == dto && c.Login == usuarioLogado.Login), default))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(retornoEsperado);

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarPropostaPareceristaConsideracaoCommand>(c =>
                c.PropostaPareceristaConsideracaoCadastroDto == dto && c.Login == usuarioLogado.Login), default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var dto = new PropostaPareceristaConsideracaoCadastroDTO
            {
                PropostaId = _faker.Random.Long(1, 100),
                Campo = CampoConsideracao.DescricaoDaAtividade,
                Descricao = _faker.Lorem.Sentence()
            };
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
