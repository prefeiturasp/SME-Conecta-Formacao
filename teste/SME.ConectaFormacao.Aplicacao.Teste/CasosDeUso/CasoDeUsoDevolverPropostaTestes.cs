using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoDevolverPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoDevolverProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoDevolverPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoDevolverProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaEJustificativaValidas_QuandoChamarExecutar_EntaoDeveDevolverPropostaEEnviarEmails()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var devolverPropostaDto = new DevolverPropostaDTO { Justificativa = _faker.Lorem.Sentence() };
            var proposta = new Proposta
            {
                Id = propostaId,
                NomeFormacao = _faker.Commerce.ProductName(),
                AreaPromotoraId = _faker.Random.Long(1)
            };
            var usuarioLogado = new Usuario { Nome = _faker.Person.FullName, Email = _faker.Internet.Email() };
            var areaPromotora = new AreaPromotora { Nome = _faker.Company.CompanyName(), Email = _faker.Internet.Email() };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), default)).ReturnsAsync(proposta);
            _mediatorMock.Setup(m => m.Send(It.IsAny<AlterarSituacaoDaPropostaCommand>(), default)).ReturnsAsync(true);
            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default)).ReturnsAsync(true);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default)).ReturnsAsync(usuarioLogado);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterAreaPromotoraPorIdQuery>(), default)).ReturnsAsync(areaPromotora);
            _mediatorMock.Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), default)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaId, devolverPropostaDto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarSituacaoDaPropostaCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.EnviarEmailDevolverProposta), default), Times.Exactly(2));
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var devolverPropostaDto = new DevolverPropostaDTO { Justificativa = _faker.Lorem.Sentence() };

            // Act
            var act = async () => await _sut.Executar(propostaId, devolverPropostaDto);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoJustificativaVazia_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var devolverPropostaDto = new DevolverPropostaDTO { Justificativa = string.Empty };
            var proposta = new Proposta { Id = propostaId };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), default)).ReturnsAsync(proposta);

            // Act
            var act = async () => await _sut.Executar(propostaId, devolverPropostaDto);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);
        }
    }
}
