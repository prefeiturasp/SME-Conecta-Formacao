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
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAprovarPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAprovarProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoAprovarPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoAprovarProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaValidaEPerfilAdminDF_QuandoChamarExecutar_EntaoDeveAprovarEPublicarNaFila()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativaDto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };
            var proposta = new Proposta { Id = propostaId, Situacao = SituacaoProposta.AguardandoAnaliseParecerPelaDF, Excluido = false };
            var perfilAdminDF = Perfis.ADMIN_DF;

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), default)).ReturnsAsync(proposta);
            _mediatorMock.Setup(m => m.Send(It.IsAny<EnviarPropostaCommand>(), default)).ReturnsAsync(true);
            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default)).ReturnsAsync(true);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(perfilAdminDF);

            // Act
            var resultado = await _sut.Executar(propostaId, justificativaDto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.NotificarAreaPromotoraSobreValidacaoFinalPelaDF), default), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativaDto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };

            // Act
            var act = async () => await _sut.Executar(propostaId, justificativaDto);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaComSituacaoInvalida_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativaDto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };
            var proposta = new Proposta { Id = propostaId, Situacao = SituacaoProposta.Publicada, Excluido = false };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), default)).ReturnsAsync(proposta);

            // Act
            var act = async () => await _sut.Executar(propostaId, justificativaDto);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECER_DF);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), default), Times.Never);
        }
    }
}
