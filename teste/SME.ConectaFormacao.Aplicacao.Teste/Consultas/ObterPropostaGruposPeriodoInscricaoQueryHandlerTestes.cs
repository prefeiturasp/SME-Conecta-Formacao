using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGruposPeriodoInscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaGruposPeriodoInscricaoQueryHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaGrupoPeriodoInscricao> _repositorioPropostaGrupoPeriodoInscricaoMock;
        private readonly ObterPropostaGruposPeriodoInscricaoQueryHandler _sut;

        public ObterPropostaGruposPeriodoInscricaoQueryHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioPropostaGrupoPeriodoInscricaoMock = autoMocker.GetMock<IRepositorioPropostaGrupoPeriodoInscricao>();
            _sut = autoMocker.CreateInstance<ObterPropostaGruposPeriodoInscricaoQueryHandler>();
        }

        [Fact]
        public async Task DadoPropostaId_QuandoHandle_EntaoDeveRetornarGruposDoRepositorio()
        {
            // Arrange
            const long propostaId = 123L;
            var gruposEsperados = new List<PropostaGrupoPeriodoInscricao>
            {
                new() { Id = 1, PropostaId = propostaId, DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1) }
            };

            _repositorioPropostaGrupoPeriodoInscricaoMock
                .Setup(r => r.ObterPorPropostaIdAsync(propostaId))
                .ReturnsAsync(gruposEsperados);

            var query = new ObterPropostaGruposPeriodoInscricaoQuery(propostaId);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(gruposEsperados);
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r => r.ObterPorPropostaIdAsync(propostaId), Times.Once);
        }
    }
}
