using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterFormacaoDetalhadaPorIdQueryHandlerTestes
    {
        private readonly Mock<ICacheDistribuido> _mockCacheDistribuido;
        private readonly Mock<IRepositorioProposta> _mockRepositorioProposta;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepositorioUsuarioAcessibilidade> _mockRepositorioUsuarioAcessibilidade;
        private readonly Mock<IRepositorioPropostaGrupoPeriodoInscricao> _mockRepositorioPropostaGrupoPeriodoInscricao;
        private readonly ObterFormacaoDetalhadaPorIdQueryHandler _handler;
        private readonly Faker _faker;

        public ObterFormacaoDetalhadaPorIdQueryHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mockCacheDistribuido = mocker.GetMock<ICacheDistribuido>();
            _mockRepositorioProposta = mocker.GetMock<IRepositorioProposta>();
            _mockMapper = mocker.GetMock<IMapper>();
            _mockRepositorioUsuarioAcessibilidade = mocker.GetMock<IRepositorioUsuarioAcessibilidade>();
            _mockRepositorioPropostaGrupoPeriodoInscricao = mocker.GetMock<IRepositorioPropostaGrupoPeriodoInscricao>();
            _handler = mocker.CreateInstance<ObterFormacaoDetalhadaPorIdQueryHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoCacheVazioEFormacaoNaoEncontrada_QuandoObter_EntaoDeveLancarExcecao()
        {
            // Arrange
            var query = new ObterFormacaoDetalhadaPorIdQuery(_faker.Random.Long(1, 100), null!);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task DadoCacheVazioEFormacaoEncontrada_QuandoObter_EntaoDeveRetornarFormacaoDetalhada()
        {
            // Arrange
            var formacaoId = _faker.Random.Long(1, 100);
            var query = new ObterFormacaoDetalhadaPorIdQuery(formacaoId, null!);
            var formacaoDetalhada = new FormacaoDetalhada
            {
                NomeFormacao = _faker.Lorem.Sentence(),
                SobreEsteCurso = _faker.Lorem.Paragraph(),
                DataInscricaoInicio = DateTime.Now.AddDays(-10),
                DataInscricaoFim = DateTime.Now.AddDays(10),
                FormacaoHomologada = FormacaoHomologada.Sim,
                ArquivoImagemDivulgacao = new() { Codigo = Guid.NewGuid(), Nome = "imagem.png" }
            };

            var acessibilidade = new UsuarioAcessibilidade
            {
                Id = _faker.Random.Long(1),
                UsuarioId = _faker.Random.Long(1),
                PossuiDeficiencia = _faker.Random.Bool()
            };

            var acessibilidadeDto = new UsuarioAcessibilidadeDto
            {
                PossuiDeficiencia = acessibilidade.PossuiDeficiencia
            };

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = formacaoDetalhada.NomeFormacao,
                SobreEsteCurso = formacaoDetalhada.SobreEsteCurso,
                FormacaoHomologada = formacaoDetalhada.FormacaoHomologada,
                UsuarioAcessibilidade = acessibilidadeDto,
                Turmas = []
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>(), false))
                .ReturnsAsync((RetornoFormacaoDetalhadaDTO)null);

            _mockRepositorioProposta
                .Setup(r => r.ObterFormacaoDetalhadaPorIdAsync(query.Id))
                .ReturnsAsync(formacaoDetalhada);

            _mockRepositorioProposta
                .Setup(r => r.ObterListagemFormacoesPorFiltro(It.IsAny<Infra.Dados.Dtos.FiltroListaFormacaoPropostaDto>()))
                .ReturnsAsync(new ResultadoPaginado<long>
                {
                    Itens = [formacaoId],
                    TotalRegistros = 1,
                    PaginaAtual = 1,
                    TamanhoPagina = 1000
                });

            _mockRepositorioUsuarioAcessibilidade
                .Setup(u => u.ObterAcessibilidadeAtualDoUsuarioAsync())
                .ReturnsAsync(acessibilidade);

            _mockMapper
                .Setup(m => m.Map<RetornoFormacaoDetalhadaDTO>(It.IsAny<FormacaoDetalhada>()))
                .Returns(formacaoDetalhadaDto);

            _mockMapper
                .Setup(m => m.Map<UsuarioAcessibilidadeDto>(It.IsAny<UsuarioAcessibilidade>()))
                .Returns(acessibilidadeDto);

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Should().BeEquivalentTo(formacaoDetalhadaDto);
            _mockCacheDistribuido
                .Verify(c => c.SalvarAsync(It.IsAny<string>(), It.IsAny<RetornoFormacaoDetalhadaDTO>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            _mockRepositorioProposta.Verify(p => p.ObterTurmasComVagaPorId(It.IsAny<long>(), null), Times.Never);
        }

        [Fact]
        public async Task DadoFormacaoNaoHomologadaNoCache_QuandoObter_EntaoDeveValidarVagasEAtualizarTurmas()
        {
            // Arrange
            var query = new ObterFormacaoDetalhadaPorIdQuery(_faker.Random.Long(1, 100), null!);
            var formacaoDetalhada = new FormacaoDetalhada
            {
                NomeFormacao = _faker.Lorem.Sentence(),
                SobreEsteCurso = _faker.Lorem.Paragraph(),
                DataInscricaoInicio = DateTime.Now.AddDays(-10),
                DataInscricaoFim = DateTime.Now.AddDays(10),
                FormacaoHomologada = FormacaoHomologada.NaoCursosPorIN,
                ArquivoImagemDivulgacao = new() { Nome = _faker.System.FilePath() }
            };

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = formacaoDetalhada.NomeFormacao,
                SobreEsteCurso = formacaoDetalhada.SobreEsteCurso,
                FormacaoHomologada = formacaoDetalhada.FormacaoHomologada,
                Turmas =
                [
                    new()
                    {
                        Id = _faker.Random.Long(1),
                        Nome = _faker.Lorem.Word(),
                        InscricaoEncerrada = false
                    }
                ]
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>(), false))
                .ReturnsAsync(formacaoDetalhadaDto);

            _mockRepositorioProposta
                .Setup(r => r.ObterTurmasComVagaPorId(query.Id, null))
                .ReturnsAsync([new() { Id = _faker.Random.Long(1) }]);

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Turmas.Should().NotBeEmpty();
            resposta.Turmas.Should().HaveCount(1);
            resposta.Turmas.First().Id.Should().Be(formacaoDetalhadaDto.Turmas.First().Id);
            resposta.Turmas.First().InscricaoEncerrada.Should().BeTrue();
            _mockCacheDistribuido
                .Verify(c => c.SalvarAsync(It.IsAny<string>(), It.IsAny<RetornoFormacaoDetalhadaDTO>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
            _mockRepositorioProposta.Verify(p => p.ObterTurmasComVagaPorId(It.IsAny<long>(), null), Times.Once);
        }

        [Fact]
        public async Task DadoTurmaComPeriodoCustomizadoExpirado_QuandoObterFormacao_EntaoNaoDeveRetornarTurmaExpirada()
        {
            // Arrange
            var formacaoId = _faker.Random.Long(1, 100);
            var query = new ObterFormacaoDetalhadaPorIdQuery(formacaoId, null!);

            var turmaExpiradaId = 101L;
            var turmaVigenteId = 102L;

            var agora = Dominio.Extensoes.DateTimeExtension.HorarioBrasilia();

            var proposta = new Proposta
            {
                Id = formacaoId,
                DataInscricaoInicio = agora.AddDays(-10),
                DataInscricaoFim = agora.AddDays(10),
                FormacaoHomologada = FormacaoHomologada.Sim
            };

            var grupoExpirado = new PropostaGrupoPeriodoInscricao
            {
                Id = 1,
                PropostaId = formacaoId,
                DataInicio = agora.AddHours(-2),
                DataFim = agora.AddMinutes(-5) // expirado!
            };
            grupoExpirado.AdicionarTurma(turmaExpiradaId);

            var grupoVigente = new PropostaGrupoPeriodoInscricao
            {
                Id = 2,
                PropostaId = formacaoId,
                DataInicio = agora.AddHours(-1),
                DataFim = agora.AddHours(2) // vigente!
            };
            grupoVigente.AdicionarTurma(turmaVigenteId);

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = "Formacao Teste",
                FormacaoHomologada = FormacaoHomologada.Sim,
                DataInscricaoFim = agora.AddDays(10),
                Turmas =
                [
                    new() { Id = turmaExpiradaId, Nome = "Turma 1 Expirada" },
                    new() { Id = turmaVigenteId, Nome = "Turma 2 Vigente" }
                ]
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>(), false))
                .ReturnsAsync(formacaoDetalhadaDto);

            _mockRepositorioProposta
                .Setup(r => r.ObterPorId(formacaoId))
                .ReturnsAsync(proposta);

            _mockRepositorioPropostaGrupoPeriodoInscricao
                .Setup(r => r.ObterPorPropostaIdAsync(formacaoId))
                .ReturnsAsync([grupoExpirado, grupoVigente]);

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Turmas.Should().HaveCount(1);
            resposta.Turmas.Single().Id.Should().Be(turmaVigenteId);
            resposta.Turmas.Any(t => t.Id == turmaExpiradaId).Should().BeFalse();
        }

        [Fact]
        public async Task DadoTodasTurmasComPeriodoCustomizadoExpirado_QuandoObterFormacao_EntaoTurmasVaziasEInscricaoEncerradaTrue()
        {
            // Arrange
            var formacaoId = _faker.Random.Long(1, 100);
            var query = new ObterFormacaoDetalhadaPorIdQuery(formacaoId, null!);
            var turmaExpiradaId = 201L;
            var agora = Dominio.Extensoes.DateTimeExtension.HorarioBrasilia();

            var proposta = new Proposta
            {
                Id = formacaoId,
                DataInscricaoInicio = agora.AddDays(-10),
                DataInscricaoFim = agora.AddDays(10),
                FormacaoHomologada = FormacaoHomologada.Sim
            };

            var grupoExpirado = new PropostaGrupoPeriodoInscricao
            {
                Id = 1,
                PropostaId = formacaoId,
                DataInicio = agora.AddHours(-3),
                DataFim = agora.AddMinutes(-10)
            };
            grupoExpirado.AdicionarTurma(turmaExpiradaId);

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = "Formacao Teste",
                FormacaoHomologada = FormacaoHomologada.Sim,
                DataInscricaoFim = agora.AddDays(10),
                Turmas = [new() { Id = turmaExpiradaId, Nome = "Turma Expirada" }]
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>(), false))
                .ReturnsAsync(formacaoDetalhadaDto);

            _mockRepositorioProposta
                .Setup(r => r.ObterPorId(formacaoId))
                .ReturnsAsync(proposta);

            _mockRepositorioPropostaGrupoPeriodoInscricao
                .Setup(r => r.ObterPorPropostaIdAsync(formacaoId))
                .ReturnsAsync([grupoExpirado]);

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Turmas.Should().BeEmpty();
            resposta.InscricaoEncerrada.Should().BeTrue();
        }

        [Fact]
        public async Task DadoTurmaSemPeriodoCustomizadoComPeriodoGeralVigente_QuandoObterFormacao_EntaoDeveRetornarTurma()
        {
            // Arrange
            var formacaoId = _faker.Random.Long(1, 100);
            var query = new ObterFormacaoDetalhadaPorIdQuery(formacaoId, null!);
            var turmaGeralId = 301L;
            var agora = Dominio.Extensoes.DateTimeExtension.HorarioBrasilia();

            var proposta = new Proposta
            {
                Id = formacaoId,
                DataInscricaoInicio = agora.AddDays(-5),
                DataInscricaoFim = agora.AddDays(5),
                FormacaoHomologada = FormacaoHomologada.Sim
            };

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = "Formacao Teste",
                FormacaoHomologada = FormacaoHomologada.Sim,
                DataInscricaoFim = agora.AddDays(5),
                Turmas = [new() { Id = turmaGeralId, Nome = "Turma Geral" }]
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>(), false))
                .ReturnsAsync(formacaoDetalhadaDto);

            _mockRepositorioProposta
                .Setup(r => r.ObterPorId(formacaoId))
                .ReturnsAsync(proposta);

            _mockRepositorioPropostaGrupoPeriodoInscricao
                .Setup(r => r.ObterPorPropostaIdAsync(formacaoId))
                .ReturnsAsync(Enumerable.Empty<PropostaGrupoPeriodoInscricao>());

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Turmas.Should().HaveCount(1);
            resposta.Turmas.Single().Id.Should().Be(turmaGeralId);
            resposta.InscricaoEncerrada.Should().BeFalse();
        }
    }
}