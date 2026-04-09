using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodo;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class SalvarPropostaGrupoPeriodoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaGrupoPeriodo> _repositorioPropostaGrupoPeriodoMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly SalvarPropostaGrupoPeriodoCommandHandler _sut;
        private readonly Faker _faker;

        public SalvarPropostaGrupoPeriodoCommandHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioPropostaGrupoPeriodoMock = autoMocker.GetMock<IRepositorioPropostaGrupoPeriodo>();
            _repositorioPropostaMock = autoMocker.GetMock<IRepositorioProposta>();
            _sut = autoMocker.CreateInstance<SalvarPropostaGrupoPeriodoCommandHandler>();
            _faker = new();
        }

        [Theory]
        [InlineData(-1, 0)] // Data de início antes da data de realização
        [InlineData(0, 1)]  // Data de fim após a data de realização
        [InlineData(-1, 1)] // Data de início antes e data de fim após a data de realização
        [InlineData(-1, -3)] // Data de início antes da data de realização e data de fim antes da data de início        
        [InlineData(3, 3)] // Data de início e data de fim após a data de realização
        [InlineData(2, -1)] // Data de início após a data de realização e data de fim antes da data de início
        public async Task DadoPeriodosInvalidos_QuandoHandle_EntaoRetornaErroValidacao(int diasParaInicio, int diasParaFim)
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = dataInicio,
                DataRealizacaoFim = dataFim,
                GruposPeriodos =
                [
                    new PropostaGrupoPeriodoDto { DataInicio = dataInicio.AddDays(diasParaInicio), DataFim = dataFim.AddDays(diasParaFim) }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoCommand(1, dto);

            var resultado = await _sut.Handle(comando, CancellationToken.None);

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
                DataRealizacaoInicio = dataInicio,
                DataRealizacaoFim = dataFim,
                GruposPeriodos =
                [
                    new PropostaGrupoPeriodoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = []
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);

            var resultado = await _sut.Handle(comando, CancellationToken.None);

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
                DataRealizacaoInicio = dataInicio,
                DataRealizacaoFim = dataFim,
                GruposPeriodos =
                [
                    new PropostaGrupoPeriodoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [0]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);

            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*identificador inválido*");
        }

        [Fact]
        public async Task DadoTurmaNaoVinculadaAProposta_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = dataInicio,
                DataRealizacaoFim = dataFim,
                GruposPeriodos =
                [
                    new PropostaGrupoPeriodoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [99]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);

            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*Uma turma não reconhecida*");
        }

        [Fact]
        public async Task DadoTurmaDuplicadaNoMesmoGrupo_QuandoHandle_EntaoRetornaErroValidacao()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = dataInicio,
                DataRealizacaoFim = dataFim,
                GruposPeriodos =
                [
                    new PropostaGrupoPeriodoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [1, 1]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1))
                .ReturnsAsync([new PropostaTurma { Id = 1, Nome = "Turma Manhã" }]);

            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().ContainMatch("*foi inserida mais de uma vez*");
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoHandle_EntaoInsereAtualizaERemoveGruposComSucesso()
        {
            // Arrange
            var dataFim = _faker.Date.Future();
            var dataInicio = dataFim.AddDays(-1);
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = dataInicio,
                DataRealizacaoFim = dataFim,
                GruposPeriodos =
                [
                    new PropostaGrupoPeriodoDto
                    {
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        PropostaTurmasIds = [1]
                    },
                    new PropostaGrupoPeriodoDto
                    {
                        Id = 1,
                        DataInicio = dataInicio.AddDays(1),
                        DataFim = dataFim,
                        PropostaTurmasIds = [2]
                    }
                ]
            };
            var comando = new SalvarPropostaGrupoPeriodoCommand(1, dto);

            _repositorioPropostaMock.Setup(r => r.ObterTurmasPorId(1))
                .ReturnsAsync(
                [
                    new PropostaTurma { Id = 1, Nome = "Turma A" },
                    new PropostaTurma { Id = 2, Nome = "Turma B" }
                ]);

            var grupoParaAtualizar = new PropostaGrupoPeriodo { Id = 1 };
            var grupoParaRemover = new PropostaGrupoPeriodo { Id = 2 };

            _repositorioPropostaGrupoPeriodoMock.Setup(r => r.ObterPorPropostaIdAsync(1))
                .ReturnsAsync([grupoParaAtualizar, grupoParaRemover]);

            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Sucesso.Should().BeTrue();

            _repositorioPropostaGrupoPeriodoMock.Verify(r =>
                r.Inserir(It.Is<PropostaGrupoPeriodo>(g => g.DataInicio == dataInicio)), Times.Once);

            _repositorioPropostaGrupoPeriodoMock.Verify(r =>
                r.Atualizar(It.Is<PropostaGrupoPeriodo>(g => g.Id == 1 && g.DataInicio == dataInicio.AddDays(1))), Times.Once);

            _repositorioPropostaGrupoPeriodoMock.Verify(r =>
                r.Atualizar(It.Is<PropostaGrupoPeriodo>(g => g.Id == 2 && g.Excluido)), Times.Once);
        }
    }
}