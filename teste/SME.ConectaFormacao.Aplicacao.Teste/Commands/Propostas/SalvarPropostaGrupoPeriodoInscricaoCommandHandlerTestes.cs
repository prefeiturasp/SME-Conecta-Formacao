using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodoInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class SalvarPropostaGrupoPeriodoInscricaoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaGrupoPeriodoInscricao> _repositorioPropostaGrupoPeriodoInscricaoMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly SalvarPropostaGrupoPeriodoInscricaoCommandHandler _sut;
        private readonly Faker _faker;

        public SalvarPropostaGrupoPeriodoInscricaoCommandHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioPropostaGrupoPeriodoInscricaoMock = autoMocker.GetMock<IRepositorioPropostaGrupoPeriodoInscricao>();
            _repositorioPropostaMock = autoMocker.GetMock<IRepositorioProposta>();
            _sut = autoMocker.CreateInstance<SalvarPropostaGrupoPeriodoInscricaoCommandHandler>();
            _faker = new();
        }

        [Theory]
        [InlineData(-1, 0)]  // Data de início antes da data geral de inscrição
        [InlineData(0, 1)]   // Data de fim após a data geral de inscrição
        [InlineData(-1, 1)]  // Fora de ambas as pontas
        [InlineData(-1, -3)] // Início antes da geral e fim antes do início
        [InlineData(3, 3)]   // Ambas após a geral
        [InlineData(2, -1)]  // Início após a geral e fim antes do início
        public async Task DadoPeriodosInvalidos_QuandoHandle_EntaoRetornaErroValidacao(int diasParaInicio, int diasParaFim)
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        DataInicio = dataInicio.AddDays(diasParaInicio),
                        DataFim = dataFim.AddDays(diasParaFim)
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("Os períodos informados não são válidos.");
        }

        [Fact]
        public async Task DadoGrupoSemTurmas_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = []
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*deve conter pelo menos uma turma vinculada*");
        }

        [Fact]
        public async Task DadoTurmaComIdInvalido_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [0]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*identificador inválido*");
        }

        [Fact]
        public async Task DadoTurmaNaoReconhecida_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [999]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*não reconhecida*");
        }

        [Fact]
        public async Task DadoTurmaDuplicadaNoMesmoGrupo_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [1, 1]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*mais de uma vez*");
        }

        [Fact]
        public async Task DadoTurmaVinculadaEmMaisDeUmGrupo_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-10);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 1,
                        DataInicio = dataInicio,
                        DataFim = dataInicio.AddDays(2),
                        PropostaTurmasIds = [1]
                    },
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 2,
                        DataInicio = dataInicio.AddDays(3),
                        DataFim = dataInicio.AddDays(5),
                        PropostaTurmasIds = [1] // Mesma turma em grupo diferente
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*já está vinculada a outro período de inscrição customizado*");
        }

        [Fact]
        public async Task DadoSemGruposInformados_QuandoHandle_EntaoRemoveGruposAnterioresERetornaSucesso()
        {
            // Arrange
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = DateTime.Today,
                DataInscricaoFim = DateTime.Today.AddDays(10),
                GruposPeriodosInscricao = []
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            var grupoAntigo = new PropostaGrupoPeriodoInscricao { Id = 10 };
            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioPropostaGrupoPeriodoInscricaoMock.Setup(r => r.ObterPorPropostaIdAsync(1))
                .ReturnsAsync([grupoAntigo]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r => r.Atualizar(It.Is<PropostaGrupoPeriodoInscricao>(g => g.Id == 10 && g.Excluido)), Times.Once);
        }

        [Fact]
        public async Task DadoGruposValidos_QuandoHandle_EntaoInsereAtualizaERemoveCorretamente()
        {
            // Arrange
            var dataInicio = DateTime.Today;
            var dataFim = DateTime.Today.AddDays(10);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicio,
                DataInscricaoFim = dataFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 1,
                        DataInicio = dataInicio.AddDays(1),
                        DataFim = dataInicio.AddDays(3),
                        PropostaTurmasIds = [1]
                    },
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = dataInicio.AddDays(4),
                        DataFim = dataInicio.AddDays(6),
                        PropostaTurmasIds = [2]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            var grupoParaAtualizar = new PropostaGrupoPeriodoInscricao { Id = 1 };
            var grupoParaRemover = new PropostaGrupoPeriodoInscricao { Id = 2 };

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" },
                new PropostaTurma { Id = 2, Nome = "Turma 2" }
            ]);

            _repositorioPropostaGrupoPeriodoInscricaoMock.Setup(r => r.ObterPorPropostaIdAsync(1))
                .ReturnsAsync([grupoParaAtualizar, grupoParaRemover]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g => g.DataInicio == dataInicio.AddDays(4))), Times.Once);
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Atualizar(It.Is<PropostaGrupoPeriodoInscricao>(g => g.Id == 1 && g.DataInicio == dataInicio.AddDays(1))), Times.Once);
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Atualizar(It.Is<PropostaGrupoPeriodoInscricao>(g => g.Id == 2 && g.Excluido)), Times.Once);
        }

        [Fact]
        public async Task DadoDataInicioGrupoIgualDataInscricaoGeral_QuandoHandle_EntaoAssumeHoraInicioGeral()
        {
            // Arrange
            var dataInicioGeral = new DateTime(2026, 11, 4, 15, 0, 0);
            var dataFimGeral = new DateTime(2026, 12, 4, 15, 0, 0);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicioGeral,
                DataInscricaoFim = dataFimGeral,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = new DateTime(2026, 11, 4, 0, 0, 0),
                        DataFim = new DateTime(2026, 11, 10, 0, 0, 0),
                        PropostaTurmasIds = [1]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g =>
                    g.DataInicio == new DateTime(2026, 11, 4, 15, 0, 0))), Times.Once);
        }

        [Fact]
        public async Task DadoDataFimGrupoIgualDataInscricaoGeral_QuandoHandle_EntaoAssumeHoraFimGeral()
        {
            // Arrange
            var dataInicioGeral = new DateTime(2026, 11, 4, 15, 0, 0);
            var dataFimGeral = new DateTime(2026, 12, 4, 15, 0, 0);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicioGeral,
                DataInscricaoFim = dataFimGeral,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = new DateTime(2026, 11, 10, 0, 0, 0),
                        DataFim = new DateTime(2026, 12, 4, 0, 0, 0),
                        PropostaTurmasIds = [1]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g =>
                    g.DataFim == new DateTime(2026, 12, 4, 15, 0, 0))), Times.Once);
        }

        [Fact]
        public async Task DadoDataInicioEFimNoMesmoDiaEHoraFimMenorQueInicio_QuandoHandle_EntaoAjustaDataFimEValidaComSucesso()
        {
            // Arrange: Grupo customizado no mesmo dia (2026-11-04) onde DataInicio assume hora 15:00 e DataFim assumiria 23:59:59
            var dataInicioGeral = new DateTime(2026, 11, 4, 15, 0, 0);
            var dataFimGeral = new DateTime(2026, 11, 10, 15, 0, 0);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicioGeral,
                DataInscricaoFim = dataFimGeral,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = new DateTime(2026, 11, 4, 0, 0, 0),
                        DataFim = new DateTime(2026, 11, 4, 0, 0, 0),
                        PropostaTurmasIds = [1]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g =>
                    g.DataInicio == new DateTime(2026, 11, 4, 15, 0, 0) &&
                    g.DataFim >= g.DataInicio)), Times.Once);
        }

        [Fact]
        public async Task DadoDataInicioEFimNoMesmoDiaComHoraFimMenorQueInicioAjustado_QuandoHandle_EntaoDataFimAssumeDataInicio()
        {
            // Arrange: Inscrição geral no mesmo dia com início e fim onde hora fim (10:00) < início (15:00)
            var dataInicioGeral = new DateTime(2026, 11, 4, 15, 0, 0);
            var dataFimGeral = new DateTime(2026, 11, 4, 10, 0, 0);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicioGeral,
                DataInscricaoFim = dataFimGeral,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = new DateTime(2026, 11, 4, 0, 0, 0),
                        DataFim = new DateTime(2026, 11, 4, 0, 0, 0),
                        PropostaTurmasIds = [1]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g =>
                    g.DataInicio == g.DataFim && g.DataInicio == new DateTime(2026, 11, 4, 15, 0, 0))), Times.Once);
        }

        [Fact]
        public async Task DadoCenarioCurlComHoraInscricaoGeralEGrupoCustomizadoSemHora_QuandoHandle_EntaoGravaComSucesso()
        {
            // Arrange exatamente como no curl reportado:
            var dataInscricaoInicio = DateTime.Parse("2026-11-04T15:00:00");
            var dataInscricaoFim = DateTime.Parse("2026-12-04T15:00:00");
            var dto = new PropostaDTO
            {
                EhProximoPasso = true,
                DataInscricaoInicio = dataInscricaoInicio,
                DataInscricaoFim = dataInscricaoFim,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = DateTime.Parse("2026-11-05"),
                        DataFim = DateTime.Parse("2026-12-02"),
                        PropostaTurmasIds = [1, 2]
                    },
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = DateTime.Parse("2026-11-04"),
                        DataFim = DateTime.Parse("2026-11-05"),
                        PropostaTurmasIds = [3, 4]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(445, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(445)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" },
                new PropostaTurma { Id = 2, Nome = "Turma 2" },
                new PropostaTurma { Id = 3, Nome = "Turma 3" },
                new PropostaTurma { Id = 4, Nome = "Turma 4" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r => r.Inserir(It.IsAny<PropostaGrupoPeriodoInscricao>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DadoExemploUsuarioComHoras14e14E14e21_QuandoHandle_EntaoGruposAssumemHorariosGerais()
        {
            // Período geral de inscrição data início 04/11/2026 14:14 data fim 04/12/2026 14:21
            // Período de inscrição customizado:
            // 1. data_inicio = 04/11/2026 14:14; data_fim = 04/11/2026 14:21
            // 2. data_inicio = 05/11/2026 14:14; data_fim = 06/11/2026 14:21.
            var dataInicioGeral = new DateTime(2026, 11, 4, 14, 14, 0);
            var dataFimGeral = new DateTime(2026, 12, 4, 14, 21, 0);
            var dto = new PropostaDTO
            {
                DataInscricaoInicio = dataInicioGeral,
                DataInscricaoFim = dataFimGeral,
                GruposPeriodosInscricao =
                [
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = new DateTime(2026, 11, 4, 0, 0, 0),
                        DataFim = new DateTime(2026, 11, 4, 0, 0, 0),
                        PropostaTurmasIds = [1]
                    },
                    new PropostaGrupoPeriodoInscricaoDto
                    {
                        Id = 0,
                        DataInicio = new DateTime(2026, 11, 5, 0, 0, 0),
                        DataFim = new DateTime(2026, 11, 6, 0, 0, 0),
                        PropostaTurmasIds = [2]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoInscricaoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1)).ReturnsAsync([
                new PropostaTurma { Id = 1, Nome = "Turma 1" },
                new PropostaTurma { Id = 2, Nome = "Turma 2" }
            ]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g =>
                    g.DataInicio == new DateTime(2026, 11, 4, 14, 14, 0) &&
                    g.DataFim == new DateTime(2026, 11, 4, 14, 21, 0))), Times.Once);

            _repositorioPropostaGrupoPeriodoInscricaoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodoInscricao>(g =>
                    g.DataInicio == new DateTime(2026, 11, 5, 14, 14, 0) &&
                    g.DataFim == new DateTime(2026, 11, 6, 14, 21, 0))), Times.Once);
        }
    }
}


