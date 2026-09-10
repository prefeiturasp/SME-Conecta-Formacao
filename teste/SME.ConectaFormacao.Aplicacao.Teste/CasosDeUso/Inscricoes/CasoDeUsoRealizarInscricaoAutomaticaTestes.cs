using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRealizarInscricaoAutomatica _sut;
        private readonly Faker _faker;

        public CasoDeUsoRealizarInscricaoAutomaticaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRealizarInscricaoAutomatica>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutar_EntaoDeveLancarExcecaoComMensagemPropostaNaoEncontrada()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var mensagem = new MensagemRabbit(propostaId.ToString());

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaInscricaoAutomaticaPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PropostaInscricaoAutomatica)null);

            // Act
            Func<Task> act = async () => await _sut.Executar(mensagem);

            // Assert
            var excecao = await act.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarPropostaTurmaVagaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaNaoPublicada_QuandoExecutar_EntaoDeveRetornarFalseSemDispararGerarPropostaTurmaVagaCommand()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var proposta = new PropostaInscricaoAutomatica
            {
                PropostaId = propostaId,
                Situacao = SituacaoProposta.Rascunho,
                TiposInscricao = new List<TipoInscricao> { TipoInscricao.Automatica }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaInscricaoAutomaticaPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit(propostaId.ToString()));

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarPropostaTurmaVagaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaNaoEhInscricaoAutomatica_QuandoExecutar_EntaoDeveRetornarFalseSemDispararGerarPropostaTurmaVagaCommand()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var proposta = new PropostaInscricaoAutomatica
            {
                PropostaId = propostaId,
                Situacao = SituacaoProposta.Publicada,
                TiposInscricao = new List<TipoInscricao> { TipoInscricao.Manual }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaInscricaoAutomaticaPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit(propostaId.ToString()));

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarPropostaTurmaVagaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPublicoAlvoSemDeParaConfigurado_QuandoExecutar_EntaoDeveLancarExcecaoCorrespondente()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var proposta = new PropostaInscricaoAutomatica
            {
                PropostaId = propostaId,
                Situacao = SituacaoProposta.Publicada,
                TiposInscricao = new List<TipoInscricao> { TipoInscricao.Automatica },
                QuantidadeVagasTurmas = 25,
                PublicosAlvos = new List<long?> { 10L, null },
                FuncoesEspecificas = new List<long?> { 20L }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaInscricaoAutomaticaPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            Func<Task> act = async () => await _sut.Executar(new MensagemRabbit(propostaId.ToString()));

            // Assert
            var excecao = await act.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_COM_PUBLICO_ALVO_SEM_DEPARA_CONFIGURADO.Parametros(propostaId));
            _mediatorMock.Verify(m => m.Send(It.Is<GerarPropostaTurmaVagaCommand>(c => c.PropostaId == propostaId && c.QuantidadeVagasTurma == 25), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterFuncionarioPorFiltroPropostaServicoEolQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoFuncaoEspecificaSemDeParaConfigurado_QuandoExecutar_EntaoDeveLancarExcecaoCorrespondente()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var proposta = new PropostaInscricaoAutomatica
            {
                PropostaId = propostaId,
                Situacao = SituacaoProposta.Publicada,
                TiposInscricao = new List<TipoInscricao> { TipoInscricao.Automatica },
                QuantidadeVagasTurmas = 30,
                PublicosAlvos = new List<long?> { 10L },
                FuncoesEspecificas = new List<long?> { null }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaInscricaoAutomaticaPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            Func<Task> act = async () => await _sut.Executar(new MensagemRabbit(propostaId.ToString()));

            // Assert
            var excecao = await act.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_COM_FUNCAO_ESPECIFICA_SEM_DEPARA_CONFIGURADO.Parametros(propostaId));
            _mediatorMock.Verify(m => m.Send(It.Is<GerarPropostaTurmaVagaCommand>(c => c.PropostaId == propostaId && c.QuantidadeVagasTurma == 30), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterFuncionarioPorFiltroPropostaServicoEolQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaValidaComPublicosEModalidades_QuandoExecutar_EntaoDeveExecutarComandosEPublicarNaFilaERetornarTrue()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var proposta = new PropostaInscricaoAutomatica
            {
                PropostaId = propostaId,
                Situacao = SituacaoProposta.Publicada,
                TiposInscricao = new List<TipoInscricao> { TipoInscricao.AutomaticaJEIF },
                QuantidadeVagasTurmas = 40,
                PublicosAlvos = new List<long?> { 101L, 102L },
                FuncoesEspecificas = new List<long?> { 201L },
                Modalidades = new List<long>
                {
                    (long)Modalidade.EducacaoInfantil,
                    (long)Modalidade.Fundamental,
                    (long)Modalidade.Medio,
                    99L
                },
                PropostasTurmas = new List<PropostaInscricaoAutomaticaTurma>
                {
                    new PropostaInscricaoAutomaticaTurma { Id = 1, CodigoDre = "DRE-01" },
                    new PropostaInscricaoAutomaticaTurma { Id = 2, CodigoDre = "DRE-02" },
                    new PropostaInscricaoAutomaticaTurma { Id = 3, CodigoDre = "DRE-01" },
                    new PropostaInscricaoAutomaticaTurma { Id = 4, CodigoDre = "" },
                    new PropostaInscricaoAutomaticaTurma { Id = 5, CodigoDre = null }
                },
                AnosTurmas = new List<string> { "2025" },
                ComponentesCurriculares = new List<long> { 501L }
            };

            var cursistasRetornados = new List<CursistaServicoEol>
            {
                new CursistaServicoEol { Rf = "1234567", Nome = "Cursista Teste", CargoDreCodigo = "DRE-01" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaInscricaoAutomaticaPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GerarPropostaTurmaVagaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterFuncionarioPorFiltroPropostaServicoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursistasRetornados);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(new MensagemRabbit(propostaId.ToString()));

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(It.Is<GerarPropostaTurmaVagaCommand>(c =>
                c.PropostaId == propostaId &&
                c.QuantidadeVagasTurma == 40
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<ObterFuncionarioPorFiltroPropostaServicoEolQuery>(q =>
                q.EhTipoJornadaJEIF == true &&
                q.CodigosCargos.SequenceEqual(new long[] { 101L, 102L }) &&
                q.CodigosFuncoes.SequenceEqual(new long[] { 201L }) &&
                q.CodigoModalidade.SequenceEqual(new long[] { 1, 10, 5, 13, 6, 9, 14, 17, 99L }) &&
                q.CodigosDres.OrderBy(d => d).SequenceEqual(new[] { "DRE-01", "DRE-02" }) &&
                q.AnosTurma.SequenceEqual(new[] { "2025" }) &&
                q.CodigosComponentesCurriculares.SequenceEqual(new[] { 501L })
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.RealizarInscricaoAutomaticaTratarTurmas &&
                c.Filtros is InscricaoAutomaticaTratarTurmasDTO &&
                ((InscricaoAutomaticaTratarTurmasDTO)c.Filtros).PropostaInscricaoAutomatica == proposta &&
                ((InscricaoAutomaticaTratarTurmasDTO)c.Filtros).CursistasEOL == cursistasRetornados
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
