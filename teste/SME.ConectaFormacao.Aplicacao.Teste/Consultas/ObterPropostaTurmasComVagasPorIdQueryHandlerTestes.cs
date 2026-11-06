using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaTurmasComVagasPorIdQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterPropostaTurmasComVagasPorIdQueryHandler _handler;

        public ObterPropostaTurmasComVagasPorIdQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<ObterPropostaTurmasComVagasPorIdQueryHandler>();
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutar_EntaoDeveLancarNegocioExceptionNotFound()
        {
            // Arrange
            var query = new ObterPropostaTurmasComVagasPorIdQuery(1, null, false);
            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPorId(query.PropostaId))
                .ReturnsAsync((Proposta?)null);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<NegocioException>();
            ex.Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            ex.Which.Message.Should().Be(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaNaoPublicadaESemTurmasComVagas_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var query = new ObterPropostaTurmasComVagasPorIdQuery(1, "123", false);
            var proposta = new Proposta
            {
                Id = 1,
                Situacao = SituacaoProposta.Rascunho,
                FormacaoHomologada = FormacaoHomologada.NaoCursosPorIN
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPorId(query.PropostaId))
                .ReturnsAsync(proposta);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterTurmasComVagaPorId(query.PropostaId, query.CodigoDre))
                .ReturnsAsync(Enumerable.Empty<PropostaTurma>());

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<NegocioException>();
            ex.Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            ex.Which.Message.Should().Be(MensagemNegocio.NENHUMA_TURMA_COM_VAGA_DISPONIVEL);
        }

        [Fact]
        public async Task DadoPropostaPublicadaEHomologada_QuandoNaoFiltrarPeriodoVigente_EntaoDeveRetornarTodasTurmas()
        {
            // Arrange
            var query = new ObterPropostaTurmasComVagasPorIdQuery(1, null, false);
            var proposta = new Proposta
            {
                Id = 1,
                Situacao = SituacaoProposta.Publicada,
                FormacaoHomologada = FormacaoHomologada.Sim
            };

            var turmas = new List<PropostaTurma>
            {
                new PropostaTurma { Id = 10, Nome = "Turma B" },
                new PropostaTurma { Id = 20, Nome = "Turma A" }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPorId(query.PropostaId))
                .ReturnsAsync(proposta);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterTurmasPorId(proposta.Id))
                .ReturnsAsync(turmas);

            _mocker.GetMock<IServicoPeriodoEncontroProposta>()
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync(" - Encontros");

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<IEnumerable<PropostaTurmaInscricaoDTO>>(It.IsAny<IEnumerable<PropostaTurma>>()))
                .Returns((IEnumerable<PropostaTurma> src) => src.Select(t => new PropostaTurmaInscricaoDTO { Id = t.Id, Descricao = t.Nome, PossuiCodafListaPresenca = t.PossuiCodafListaPresenca }));

            // Act
            var resultado = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado[0].Descricao.Should().Be("Turma A - Encontros");
            resultado[1].Descricao.Should().Be("Turma B - Encontros");
            _mocker.GetMock<IRepositorioPropostaGrupoPeriodoInscricao>()
                .Verify(r => r.ObterPorPropostaIdAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoApenasPeriodoInscricaoVigenteVerdadeiro_QuandoExecutar_EntaoDeveFiltrarTurmasForaDoPeriodo()
        {
            // Arrange
            var query = new ObterPropostaTurmasComVagasPorIdQuery(1, null, true);
            var proposta = new Proposta
            {
                Id = 1,
                Situacao = SituacaoProposta.Publicada,
                FormacaoHomologada = FormacaoHomologada.Sim,
                DataInscricaoInicio = DateTime.Today.AddDays(-5),
                DataInscricaoFim = DateTime.Today.AddDays(5)
            };

            var turmaDentroGlobal = new PropostaTurma { Id = 1, Nome = "Turma Global" };
            var turmaComPeriodoCustomizadoVigente = new PropostaTurma { Id = 2, Nome = "Turma Custom Vigente" };
            var turmaComPeriodoCustomizadoExpirado = new PropostaTurma { Id = 3, Nome = "Turma Custom Expirada" };

            var grupoVigente = new PropostaGrupoPeriodoInscricao
            {
                Id = 10,
                DataInicio = DateTime.Today.AddDays(-2),
                DataFim = DateTime.Today.AddDays(2)
            };
            grupoVigente.AdicionarTurma(turmaComPeriodoCustomizadoVigente.Id);

            var grupoExpirado = new PropostaGrupoPeriodoInscricao
            {
                Id = 20,
                DataInicio = DateTime.Today.AddDays(-10),
                DataFim = DateTime.Today.AddDays(-5)
            };
            grupoExpirado.AdicionarTurma(turmaComPeriodoCustomizadoExpirado.Id);

            var turmas = new List<PropostaTurma> { turmaDentroGlobal, turmaComPeriodoCustomizadoVigente, turmaComPeriodoCustomizadoExpirado };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPorId(query.PropostaId))
                .ReturnsAsync(proposta);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterTurmasPorId(proposta.Id))
                .ReturnsAsync(turmas);

            _mocker.GetMock<IRepositorioPropostaGrupoPeriodoInscricao>()
                .Setup(r => r.ObterPorPropostaIdAsync(query.PropostaId))
                .ReturnsAsync(new[] { grupoVigente, grupoExpirado });

            _mocker.GetMock<IServicoPeriodoEncontroProposta>()
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync("");

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<IEnumerable<PropostaTurmaInscricaoDTO>>(It.IsAny<IEnumerable<PropostaTurma>>()))
                .Returns((IEnumerable<PropostaTurma> src) => src.Select(t => new PropostaTurmaInscricaoDTO { Id = t.Id, Descricao = t.Nome, PossuiCodafListaPresenca = t.PossuiCodafListaPresenca }));

            // Act
            var resultado = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Select(x => x.Id).Should().Contain(new long[] { 1, 2 });
            resultado.Select(x => x.Id).Should().NotContain(3);
        }

        [Fact]
        public async Task DadoTurmas_QuandoExecutar_EntaoDeveRetornarPossuiCodafListaPresencaCorretamente()
        {
            // Arrange
            var query = new ObterPropostaTurmasComVagasPorIdQuery(1, null, false);
            var proposta = new Proposta
            {
                Id = 1,
                Situacao = SituacaoProposta.Publicada,
                FormacaoHomologada = FormacaoHomologada.Sim
            };

            var turmas = new List<PropostaTurma>
            {
                new() { Id = 10, Nome = "Turma Com Codaf", PossuiCodafListaPresenca = true },
                new() { Id = 20, Nome = "Turma Sem Codaf", PossuiCodafListaPresenca = false }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPorId(query.PropostaId))
                .ReturnsAsync(proposta);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterTurmasPorId(proposta.Id))
                .ReturnsAsync(turmas);

            _mocker.GetMock<IServicoPeriodoEncontroProposta>()
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync("");

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<IEnumerable<PropostaTurmaInscricaoDTO>>(It.IsAny<IEnumerable<PropostaTurma>>()))
                .Returns((IEnumerable<PropostaTurma> src) => src.Select(t => new PropostaTurmaInscricaoDTO { Id = t.Id, Descricao = t.Nome, PossuiCodafListaPresenca = t.PossuiCodafListaPresenca }));

            // Act
            var resultado = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            var turmaComCodaf = resultado.First(x => x.Id == 10);
            var turmaSemCodaf = resultado.First(x => x.Id == 20);

            turmaComCodaf.PossuiCodafListaPresenca.Should().BeTrue();
            turmaSemCodaf.PossuiCodafListaPresenca.Should().BeFalse();
        }
    }
}
