using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterDashBoardQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICacheDistribuido> _cacheMock;
        private readonly ObterDashBoardQueryHandler _handler;
        private readonly Faker _faker;

        public ObterDashBoardQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _cacheMock = _mocker.GetMock<ICacheDistribuido>();
            _handler = _mocker.CreateInstance<ObterDashBoardQueryHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoPropostasComParteNoCacheEParteNoBanco_QuandoHandle_EntaoDeveMontarDashboardESalvarNoCache()
        {
            // Arrange
            var query = new ObterDashBoardQuery(new PropostaFiltrosDashboardDTO(), null);
            var propostasIds = Enumerable.Range(1, 6)
                .Select(id => CriarProposta(id, SituacaoProposta.Publicada))
                .ToList();

            var propostaCache = CriarProposta(1, SituacaoProposta.Publicada);
            var propostasBanco = propostasIds.Skip(1).Take(4).ToArray();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostasIdDashboardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostasDashboardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasBanco);

            _cacheMock
                .SetupSequence(c => c.ObterObjetoAsync<Proposta>(It.IsAny<string>(), false))
                .ReturnsAsync(propostaCache)
                .ReturnsAsync((Proposta)null!)
                .ReturnsAsync((Proposta)null!)
                .ReturnsAsync((Proposta)null!)
                .ReturnsAsync((Proposta)null!);

            _cacheMock
                .Setup(c => c.SalvarAsync(It.IsAny<string>(), It.IsAny<Proposta>(), 720, false))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            resultado.Should().ContainSingle();
            resultado[0].Situacao.Should().Be(SituacaoProposta.Publicada);
            resultado[0].Propostas.Should().HaveCount(5);
            resultado[0].TotalRegistros.Should().Be("1");

            _cacheMock.Verify(c => c.SalvarAsync(It.IsAny<string>(), It.IsAny<Proposta>(), 720, false), Times.Exactly(4));
        }

        [Fact]
        public async Task DadoFiltroPorSituacaoSemPropostas_QuandoHandle_EntaoDeveRetornarListaVazia()
        {
            // Arrange
            var query = new ObterDashBoardQuery(new PropostaFiltrosDashboardDTO
            {
                Situacao = SituacaoProposta.Recusada
            }, null);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostasIdDashboardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Proposta>());

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().BeEmpty();
            _cacheMock.Verify(c => c.ObterObjetoAsync<Proposta>(It.IsAny<string>(), false), Times.Never);
        }

        private Proposta CriarProposta(long id, SituacaoProposta situacao)
        {
            return new Proposta
            {
                Id = id,
                NomeFormacao = _faker.Lorem.Sentence(3),
                Situacao = situacao,
                CriadoEm = DateTime.Today,
                AlteradoEm = DateTime.Today
            };
        }
    }
}
