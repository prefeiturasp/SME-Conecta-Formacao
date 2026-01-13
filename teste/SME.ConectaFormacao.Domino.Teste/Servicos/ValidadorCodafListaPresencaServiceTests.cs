using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.Domino.Teste.Servicos
{
    public class ValidadorCodafListaPresencaServiceTests
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioListaMock;
        private readonly Mock<IRepositorioCodafInscritosListaPresenca> _repositorioInscritosMock;
        private readonly ValidadorCodafListaPresencaService _validadorService;
        private readonly Faker _faker;

        public ValidadorCodafListaPresencaServiceTests()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _repositorioListaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioInscritosMock = mocker.GetMock<IRepositorioCodafInscritosListaPresenca>();
            _validadorService = mocker.CreateInstance<ValidadorCodafListaPresencaService>();
            _faker = new("pt_BR");
        }

        [Fact]
        public async Task DadoUmaTurmaComListaDePresenca_QuandoValidarUnicidadeTurmaListaDePresenca_EntaoDeveRetornarErroDeNegocio()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioListaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, It.IsAny<long>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _validadorService.ValidarUnicidadeTurmaListaDePresencaAsync(propostaTurmaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("Já existe uma lista de presença cadastrada para esta turma.");
        }

        [Fact]
        public async Task DadoUmaTurmaSemListaDePresenca_QuandoValidarUnicidadeTurmaListaDePresenca_EntaoNaoDeveRetornarErro()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioListaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, It.IsAny<long>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _validadorService.ValidarUnicidadeTurmaListaDePresencaAsync(propostaTurmaId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task DadoUmaPropostaInexistente_QuandoValidarVinculoPropostaTurma_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("Proposta não encontrada.");
        }

        [Fact]
        public async Task DadoUmaTurmaInexistente_QuandoValidarVinculoPropostaTurma_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta());

            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("Turma não encontrada.");
        }

        [Fact]
        public async Task DadoUmaTurmaDeOutraProposta_QuandoValidarVinculoPropostaTurma_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaNaoExcluidaPorIdAsync(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId + 1 });

            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("A turma informada não pertence à formação selecionada.");
        }

        [Fact]
        public async Task DadoUmaTurmaValida_QuandoValidarVinculoPropostaTurma_EntaoNaoDeveRetornarErro()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaNaoExcluidaPorIdAsync(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);
            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task DadoUmaListaDePresencaComDataDePublicacaoNull_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListaPresenca = new CodafListaPresenca(_faker.Random.Long(1), _faker.Random.Long(1),
                null, null, null, null, null, null, null, null);
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("A data de publicação da lista de presença é obrigatória.");
        }

        [Fact]
        public async Task DadoUmaListaDePresencaComNumeroComunicadoNull_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListaPresenca = new CodafListaPresenca(_faker.Random.Long(1), _faker.Random.Long(1), _faker.Date.Recent(),
                 null, null, null, null, null, null, null);
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O número do comunicado DOM é obrigatório.");
        }

        [Fact]
        public async Task DadoUmaListaDePresencaComPaginaComunicadoDomNull_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListaPresenca = new CodafListaPresenca(_faker.Random.Long(1), _faker.Random.Long(1), _faker.Date.Recent(),
                 null, 2, null, null, null, null, null);
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("A página do comunicado DOM é obrigatória.");
        }

        [Fact]
        public async Task DadoUmaTurmaComListaDePresenca_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeNegocio()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(_faker.Random.Long(1), propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null);
            _repositorioListaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, codafListaPresenca.Id))
                .ReturnsAsync(true);
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("Já existe uma lista de presença cadastrada para esta turma.");
        }

        [Fact]
        public async Task DadoUmaTurmaDeOutraProposta_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null);
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId + 1 });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("A turma informada não pertence à formação selecionada.");
        }

        [Fact]
        public async Task DadoUmaListaSemAnexo_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null);
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("É obrigatório o envio de ao menos um anexo para a lista de presença.");
        }

        [Fact]
        public async Task DadoUmaListaSemInscritos_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ]
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 0,
                    Itens = []
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("Não é possível enviar a lista de presença para o DF sem inscritos.");
        }

        [Fact]
        public async Task DadoUmaListaComQuantidadeDeInscritosDivergentes_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                Proposta = new() { NomeFormacao = _faker.Lorem.Sentence() }
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = []
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain($"Há divergência entre a quantidade de inscritos na formação {codafListaPresenca.Proposta?.NomeFormacao} e a lista de presença.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosSemPercentualDeFrequencia_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = null }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [ new() { PercentualFrequencia = null } ]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O percentual de frequência de todas as inscrições deve estar entre 0 e 100.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosComPercentualAbaixoDoRange_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(decimal.MinValue, -0.001m) }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O percentual de frequência de todas as inscrições deve estar entre 0 e 100.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosComPercentualAcimaDoRange_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(100.001m, decimal.MaxValue) }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O percentual de frequência de todas as inscrições deve estar entre 0 e 100.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosSemAtividadeObrigatoria_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(0, 100), AtividadeObrigatorio = null }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O campo 'Atividade Obrigatório' deve ser preenchido para todas as inscrições.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosSemConceitoFinal_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(0, 100), AtividadeObrigatorio = false, ConceitoFinal = null }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O campo 'Conceito Final' deve ser preenchido corretamente para todas as inscrições.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosConceitoFinalIncorreto_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(0, 100), AtividadeObrigatorio = false, ConceitoFinal = "a" }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O campo 'Conceito Final' deve ser preenchido corretamente para todas as inscrições.");
        }

        [Fact]
        public async Task DadoUmaListaComInscritosSemAprovadoInformado_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(0, 100), AtividadeObrigatorio = false, ConceitoFinal = "P", Aprovado = null }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });
            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("O campo 'Aprovado' deve ser preenchido para todas as inscrições.");
        }

        [Fact]
        public async Task DadoUmaListaValida_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarNull()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { PercentualFrequencia = _faker.Random.Decimal(0, 100), AtividadeObrigatorio = false, ConceitoFinal = "S", Aprovado = true }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { PercentualFrequencia = null }]
                });

            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task DadoUmaListaComInscritosDiferenteDaTurma_QuandoValidarParaEnvioAoDf_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var inscritoId = _faker.Random.Long(1);
            var codafListaPresenca = new CodafListaPresenca(propostaId, propostaTurmaId, _faker.Date.Recent(),
                 null, 2, 5, null, null, null, null)
            {
                CodafAnexos =
                [
                    new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = _faker.System.FileName(), Extensao = _faker.System.FileExt(), TipoAnexoId = _faker.PickRandom<TipoAnexoCodaf>() }
                ],
                CodafInscricoes = [new() { Id = inscritoId, PercentualFrequencia = _faker.Random.Decimal(0, 100), AtividadeObrigatorio = false, ConceitoFinal = "NS", Aprovado = true }]
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            _repositorioInscritosMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    TotalRegistros = 1,
                    Itens = [new() { Id = inscritoId+1 }]
                });

            // Act
            var resultado = await _validadorService.ValidarParaEnvioAoDfAsync(codafListaPresenca);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain($"Há divergência entre a quantidade de inscritos na formação {codafListaPresenca.Proposta?.NomeFormacao} e a lista de presença.");
        }
    }
}