using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGrupoPeriodoPorPropostaId;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaGrupoPeriodoPorPropostaIdQueryHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaGrupoPeriodo> _repositorioPropostaGrupoPeriodoMock;
        private readonly ObterPropostaGrupoPeriodoPorPropostaIdQueryHandler _sut;
        private readonly Faker _faker;

        public ObterPropostaGrupoPeriodoPorPropostaIdQueryHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioPropostaGrupoPeriodoMock = autoMocker.GetMock<IRepositorioPropostaGrupoPeriodo>();
            _sut = autoMocker.CreateInstance<ObterPropostaGrupoPeriodoPorPropostaIdQueryHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoGruposExistentes_QuandoHandle_EntaoMapeiaERetornaDtosCorretamente()
        {
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var propostaId = 10L;
            var grupoPeriodo = new PropostaGrupoPeriodo
            {
                Id = 1,
                PropostaId = propostaId,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            grupoPeriodo.AdicionarTurma(5);
            grupoPeriodo.AdicionarTurma(8);

            _repositorioPropostaGrupoPeriodoMock
                .Setup(r => r.ObterPorPropostaIdAsync(propostaId))
                .ReturnsAsync([grupoPeriodo]);

            var query = new ObterPropostaGrupoPeriodoPorPropostaIdQuery(propostaId);

            var resultado = await _sut.Handle(query, CancellationToken.None);

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
            var propostaId = 99L;

            _repositorioPropostaGrupoPeriodoMock
                .Setup(r => r.ObterPorPropostaIdAsync(propostaId))
                .ReturnsAsync([]);

            var query = new ObterPropostaGrupoPeriodoPorPropostaIdQuery(propostaId);

            var resultado = await _sut.Handle(query, CancellationToken.None);

            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }
    }
}
