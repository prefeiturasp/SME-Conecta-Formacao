using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAtualizarCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioCodafInscritosListaPresenca> _repositorioCodafInscritosListaPresencaMock;
        private readonly Mock<IRepositorioCodafRetificacaoListaPresenca> _repositorioCodafRetificacaoListaPresencaMock;
        private readonly Mock<IValidadorCodafListaPresencaService> _validadorCodafListaPresencaServiceMock;
        private readonly Mock<IValidator<CodafListaPresencaEdicaoDto>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly CasoDeUsoAtualizarCodafListaPresenca _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoAtualizarCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafInscritosListaPresencaMock = mocker.GetMock<IRepositorioCodafInscritosListaPresenca>();
            _repositorioCodafRetificacaoListaPresencaMock = mocker.GetMock<IRepositorioCodafRetificacaoListaPresenca>();
            _validadorCodafListaPresencaServiceMock = mocker.GetMock<IValidadorCodafListaPresencaService>();
            _mapperMock = mocker.GetMock<IMapper>();
            _validatorMock = mocker.GetMock<IValidator<CodafListaPresencaEdicaoDto>>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoAtualizarCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoCodafListPresencaInexistente_QuandoExecutar_EntaoDeveErroNaoEncontradoENaoChamarAtualizar()
        {
            // Arrange
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = _faker.Random.Long(1, long.MaxValue),
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Int(1, int.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Lista de presença não encontrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmDtoInvalido_QuandoChamarExecutar_EntaoDeveRetornarErroValidacaoENaoChamarAtualizar()
        {
            // Arrange
            var propostaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdInvalido,
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdInvalido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                [
                    new FluentValidation.Results.ValidationFailure("PropostaId", "PropostaId inválido."),
                    new FluentValidation.Results.ValidationFailure("PropostaTurmaId", "PropostaTurmaId inválido.")
                ]));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("PropostaId inválido.");
            resultado.MensagensErro.Should().Contain("PropostaTurmaId inválido.");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);

        }

        [Fact]
        public async Task DadoErroDeVinculo_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarAtualizar()
        {
            // Arrange
            var propostaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdInvalido,
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };
            var mensagemErroVinculo = _faker.Lorem.Sentence();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdInvalido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _validadorCodafListaPresencaServiceMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Erro.Validacao(mensagemErroVinculo));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain(mensagemErroVinculo);
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroUnicidadeTurma_QuandoExecutar_EntaoDeveRetornarErroValidacao()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdInvalido
            };
            var mensagemErroVinculo = _faker.Lorem.Sentence();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _validadorCodafListaPresencaServiceMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Erro.Validacao(mensagemErroVinculo));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain(mensagemErroVinculo);
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
        }

        [Fact]
        public async Task DadoEdicaoSemInscritos_QuandoExecutar_EntaoDeveAtualizarDadosDaListaDePresenca()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null)
            {
                Id = _faker.Random.Long(1, long.MaxValue)
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mapperMock
                .Setup(m => m.Map<List<CodafInscricaoListaPresenca>>(It.IsAny<List<CodafInscritoListaPresencaSalvarDto>>()))
                .Returns([]);

            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.Is<CodafListaPresenca>(c =>
                c.PropostaId == propostaIdValido &&
                c.PropostaTurmaId == propostaTurmaIdValido
            )), Times.Once);
            _repositorioCodafInscritosListaPresencaMock.Verify(r => r.ExcluirPorListaPresencaIdAsync(codafListaPresencaExistente.Id), Times.Once);
            _repositorioCodafInscritosListaPresencaMock.Verify(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafInscricaoListaPresenca>>()), Times.Never);
            transacaoMock.Verify(t => t.Commit(), Times.Once);
            transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public async Task DadoEdicaoComInscritos_QuandoExecutar_EntaoDeveAtualizarDadosDaListaDePresencaEInscritos()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Inscritos = [
                    new (){
                        Aprovado = true,
                        AtividadeObrigatorio = true,
                        ConceitoFinal = "S",
                        InscricaoId = _faker.Random.Long(1, long.MaxValue),
                        PercentualFrequencia = 95.5m
                    }
                ]
            };
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null)
            {
                Id = _faker.Random.Long(1, long.MaxValue)
            };
            var inscritos = new List<CodafInscricaoListaPresenca>
            {
                new() {
                    InscricaoId = codafListaPresencaEdicaoDto.Inscritos[0].InscricaoId,
                    Aprovado = codafListaPresencaEdicaoDto.Inscritos[0].Aprovado,
                    AtividadeObrigatorio = codafListaPresencaEdicaoDto.Inscritos[0].AtividadeObrigatorio,
                    ConceitoFinal = codafListaPresencaEdicaoDto.Inscritos[0].ConceitoFinal,
                    PercentualFrequencia = codafListaPresencaEdicaoDto.Inscritos[0].PercentualFrequencia
                }
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock
                .Setup(m => m.Map<List<CodafInscricaoListaPresenca>>(It.IsAny<List<CodafInscritoListaPresencaSalvarDto>>()))
                .Returns(inscritos);
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.Is<CodafListaPresenca>(c =>
                c.PropostaId == propostaIdValido &&
                c.PropostaTurmaId == propostaTurmaIdValido
            )), Times.Once);
            _repositorioCodafInscritosListaPresencaMock.Verify(r => r.ExcluirPorListaPresencaIdAsync(codafListaPresencaExistente.Id), Times.Once);
            _repositorioCodafInscritosListaPresencaMock.Verify(r => r.InserirVariosAsync(It.Is<IEnumerable<CodafInscricaoListaPresenca>>(i =>
                i.Count() == inscritos.Count &&
                i.First().InscricaoId == inscritos.First().InscricaoId &&
                i.First().Aprovado == inscritos.First().Aprovado &&
                i.First().AtividadeObrigatorio == inscritos.First().AtividadeObrigatorio &&
                i.First().ConceitoFinal == inscritos.First().ConceitoFinal &&
                i.First().PercentualFrequencia == inscritos.First().PercentualFrequencia
            )), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Once);
            transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public async Task DadoEdicaoComNovasRetificacoes_QuandoExecutar_EntaoDeveAtualizarDadosDaListaDePresencaERetificacoes()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Retificacoes = [
                    new (){
                        DataRetificacao = DateTime.Now,
                        PaginaRetificacaoDom = 5,
                    }
                ]
            };
            var codafRetificacoes = new List<CodafRetificacaoListaPresenca>
            {
                new() {
                    DataRetificacao = codafListaPresencaEdicaoDto.Retificacoes[0].DataRetificacao,
                    PaginaRetificacaoDom = codafListaPresencaEdicaoDto.Retificacoes[0].PaginaRetificacaoDom
                }
            };
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null)
            {
                Id = _faker.Random.Long(1, long.MaxValue)
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _mapperMock
                .Setup(m => m.Map<CodafRetificacaoListaPresenca>(It.IsAny<CodafRetificacaoListaPresencaSalvarDto>()))
                .Returns(codafRetificacoes[0]);

            // Act
            await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Inserir(It.Is<CodafRetificacaoListaPresenca>(i =>
                i.DataRetificacao == codafRetificacoes.First().DataRetificacao &&
                i.PaginaRetificacaoDom == codafRetificacoes.First().PaginaRetificacaoDom
            )), Times.Once);
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Remover(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoEdicaoComRetificacoesAtualizadas_QuandoExecutar_EntaoDeveAtualizarDadosDaListaDePresencaERetificacoes()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Retificacoes = [
                    new (){
                        Id = _faker.Random.Long(1, long.MaxValue),
                        DataRetificacao = DateTime.Now,
                        PaginaRetificacaoDom = 10,
                    }
                ]
            };
            var codafRetificacoes = new List<CodafRetificacaoListaPresenca>
            {
                new() {
                    Id = codafListaPresencaEdicaoDto.Retificacoes[0].Id,
                    DataRetificacao = codafListaPresencaEdicaoDto.Retificacoes[0].DataRetificacao,
                    PaginaRetificacaoDom = codafListaPresencaEdicaoDto.Retificacoes[0].PaginaRetificacaoDom
                }
            };
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null)
            {
                Id = _faker.Random.Long(1, long.MaxValue)
            };
            var codafRetificacoesExistentes = new List<CodafRetificacaoListaPresenca>
            {
                new()
                {
                    Id = codafListaPresencaEdicaoDto.Retificacoes[0].Id,
                    CodafListaPresencaId = codafListaPresencaExistente.Id,
                    DataRetificacao = DateTime.Now.AddDays(-10),
                    PaginaRetificacaoDom = 3
                }
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _mapperMock
                .Setup(m => m.Map<CodafRetificacaoListaPresenca>(It.IsAny<CodafRetificacaoListaPresencaSalvarDto>()))
                .Returns(codafRetificacoes[0]);
            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterPorListaPresencaIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafRetificacoesExistentes);

            // Act
            await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            _validatorMock
                .Verify(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default), Times.Once);
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Atualizar(It.Is<CodafRetificacaoListaPresenca>(i =>
                i.Id == codafRetificacoes.First().Id &&
                i.DataRetificacao == codafRetificacoes.First().DataRetificacao &&
                i.PaginaRetificacaoDom == codafRetificacoes.First().PaginaRetificacaoDom
            )), Times.Once);
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafRetificacaoListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoEdicaoComRetificacoesRemovidas_QuandoExecutar_EntaoDeveAtualizarDadosDaListaDePresencaERetificacoes()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Retificacoes = []
            };
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null)
            {
                Id = _faker.Random.Long(1, long.MaxValue)
            };
            var codafRetificacoesExistentes = new List<CodafRetificacaoListaPresenca>
            {
                new()
                {
                    Id = _faker.Random.Long(1, long.MaxValue),
                    CodafListaPresencaId = codafListaPresencaExistente.Id,
                    DataRetificacao = DateTime.Now.AddDays(-10),
                    PaginaRetificacaoDom = 3
                }
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterPorListaPresencaIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafRetificacoesExistentes);
         
            // Act
            await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Remover(It.Is<CodafRetificacaoListaPresenca>(id =>
                id == codafRetificacoesExistentes.First()
            )), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoAtualizar_QuandoExecutar_EntaoDeveRetornarErroDeFalhaEChamarRollback()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null));
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _repositorioCodafListaPresencaMock.Setup(r => r.Atualizar(It.IsAny<CodafListaPresenca>()))
                .ThrowsAsync(new Exception("Erro ao atualizar"));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Erro ao atualizar a lista de presença.");
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Never);
        }
    }
}