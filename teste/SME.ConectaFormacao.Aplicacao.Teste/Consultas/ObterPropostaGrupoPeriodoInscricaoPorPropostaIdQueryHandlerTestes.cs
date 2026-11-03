using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGrupoPeriodoInscricaoPorPropostaId;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQueryHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaGrupoPeriodoInscricao> _repositorioPropostaGrupoPeriodoInscricaoMock;
        private readonly ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQueryHandler _sut;
        private readonly Faker _faker;

        public ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQueryHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioPropostaGrupoPeriodoInscricaoMock = autoMocker.GetMock<IRepositorioPropostaGrupoPeriodoInscricao>();
            _sut = autoMocker.CreateInstance<ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQueryHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoGruposExistentes_QuandoHandle_EntaoMapeiaERetornaDtosCorretamente()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var propostaId = 10L;
            var grupoPeriodo = new PropostaGrupoPeriodoInscricao
            {
                Id = 1,
                PropostaId = propostaId,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            grupoPeriodo.AdicionarTurma(5);
            grupoPeriodo.AdicionarTurma(8);

            _repositorioPropostaGrupoPeriodoInscricaoMock
                .Setup(r => r.ObterPorPropostaIdAsync(propostaId))
                .ReturnsAsync([grupoPeriodo]);

            var query = new ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQuery(propostaId);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);

            var dto = resultado.First();
            dto.Id.Should().Be(1);
            dto.DataInicio.Should().Be(dataInicio);
            dto.DataFim.Should().Be(dataFim);
            dto.PropostaTurmasIds.Should().BeEquivalentTo([5L, 8L]);
        }

        [Fact]
        public async Task DadoNenhumGrupoEncontrado_QuandoHandle_EntaoRetornaListaVazia()
        {
            // Arrange
            var propostaId = 99L;

            _repositorioPropostaGrupoPeriodoInscricaoMock
                .Setup(r => r.ObterPorPropostaIdAsync(propostaId))
                .ReturnsAsync([]);

            var query = new ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQuery(propostaId);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }
    }
}
