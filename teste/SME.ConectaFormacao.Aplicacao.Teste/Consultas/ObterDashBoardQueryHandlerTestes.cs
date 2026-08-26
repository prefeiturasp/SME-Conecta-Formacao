using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterDashBoardQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterDashBoardQueryHandler _sut;

        public ObterDashBoardQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterDashBoardQueryHandler>();
        }

        [Fact]
        public async Task DadoObterDashboard_QuandoExecutar_EntaoRetornaPropostas()
        {
            // Arrange
            var filtros = new PropostaFiltrosDashboardDTO
            {
                Situacao = SituacaoProposta.Rascunho
            };

            var request = new ObterDashBoardQuery(filtros, 1);

            var propostasIdDashboard = new List<Proposta>
            {
                new() { Id = 1, Situacao = SituacaoProposta.Rascunho },
                new() { Id = 2, Situacao = SituacaoProposta.Rascunho },
                new() { Id = 3, Situacao = SituacaoProposta.Rascunho },
                new() { Id = 4, Situacao = SituacaoProposta.Rascunho },
                new() { Id = 5, Situacao = SituacaoProposta.Rascunho },
                new() { Id = 6, Situacao = SituacaoProposta.Rascunho }
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostasIdDashboardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIdDashboard);

            var propostasCompletas = new List<Proposta>();
            for (int i = 1; i <= 5; i++)
            {
                propostasCompletas.Add(new Proposta { Id = i, NomeFormacao = $"Proposta {i}", CriadoEm = DateTime.Today });
            }

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostasDashboardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasCompletas);

            // Act
            var resultado = await _sut.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1); // Apenas Rascunho porque foi filtrado

            var dashboardRascunho = resultado.First();
            dashboardRascunho.Situacao.Should().Be(SituacaoProposta.Rascunho);
            dashboardRascunho.TotalRegistros.Should().Be("1"); // 6 total - 5 = 1 no "Ver mais"
            dashboardRascunho.Propostas.Should().HaveCount(5); // Pega apenas os 5 primeiros (Take(5))

            var primeiroItem = dashboardRascunho.Propostas[0];
            primeiroItem.Numero.Should().Be(1);
            primeiroItem.Nome.Should().Be("Proposta 1");
        }

        [Fact]
        public async Task DadoObterDashboardComCachePreenchido_QuandoExecutar_EntaoNaoBuscaNoBanco()
        {
            // Arrange
            var filtros = new PropostaFiltrosDashboardDTO();

            var request = new ObterDashBoardQuery(filtros, 1);

            var propostasIdDashboard = new List<Proposta>
            {
                new() { Id = 1, Situacao = SituacaoProposta.Aprovada }
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostasIdDashboardQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIdDashboard);

            // Simula o cache TENDO a proposta
            _mocker.GetMock<ICacheDistribuido>()
                .Setup(m => m.ObterObjetoAsync<Proposta>(It.IsAny<string>()))
                .ReturnsAsync(new Proposta { Id = 1, NomeFormacao = "Proposta Cache", CriadoEm = DateTime.Today });

            // Act
            var resultado = await _sut.Handle(request, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);

            var dashboardAprovada = resultado.First();
            dashboardAprovada.Situacao.Should().Be(SituacaoProposta.Aprovada);
            dashboardAprovada.TotalRegistros.Should().BeEmpty(); // 1 total - 5 < 0, então empty
            dashboardAprovada.Propostas.Should().HaveCount(1);

            var primeiroItem = dashboardAprovada.Propostas[0];
            primeiroItem.Numero.Should().Be(1);
            primeiroItem.Nome.Should().Be("Proposta Cache");

            // Não deve ter chamado o mediator para buscar completa no banco
            _mocker.GetMock<IMediator>()
                .Verify(m => m.Send(It.IsAny<ObterPropostasDashboardQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
