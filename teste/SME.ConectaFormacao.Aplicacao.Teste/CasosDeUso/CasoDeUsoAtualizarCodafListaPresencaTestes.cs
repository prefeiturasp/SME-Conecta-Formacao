using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAtualizarCodafListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioCodafRetificacaoListaPresenca> _repositorioCodafRetificacaoListaPresencaMock;
        private readonly Mock<ICodafInscritosListaPresencaService> _inscritosServiceMock;
        private readonly Mock<IValidadorCodafListaPresencaService> _validadorDominioMock;
        private readonly Mock<IGerenciadorAnexosCodafService> _anexosServiceMock;
        private readonly Mock<IGerenciadorMovimentacaoCodafService> _movimentacaoServiceMock;

        private readonly Mock<IValidator<CodafListaPresencaEdicaoDto>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoAtualizarCodafListaPresenca _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoAtualizarCodafListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafRetificacaoListaPresencaMock = mocker.GetMock<IRepositorioCodafRetificacaoListaPresenca>();
            _inscritosServiceMock = mocker.GetMock<ICodafInscritosListaPresencaService>();
            _validadorDominioMock = mocker.GetMock<IValidadorCodafListaPresencaService>();
            _anexosServiceMock = mocker.GetMock<IGerenciadorAnexosCodafService>();
            _movimentacaoServiceMock = mocker.GetMock<IGerenciadorMovimentacaoCodafService>();
            var dependencias = new CodafListaPresencaDependencias(
                _repositorioCodafListaPresencaMock.Object,
                _repositorioCodafRetificacaoListaPresencaMock.Object,
                _inscritosServiceMock.Object,
                _validadorDominioMock.Object,
                _anexosServiceMock.Object,
                _movimentacaoServiceMock.Object
            );
            mocker.Use(dependencias);
            _validatorMock = mocker.GetMock<IValidator<CodafListaPresencaEdicaoDto>>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoAtualizarCodafListaPresenca>();
            _faker = new();

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid());
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
                .ReturnsAsync(new CodafListaPresenca(propostaIdInvalido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, new(null, null, null, null, null, null, null), null));

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
                .ReturnsAsync(new CodafListaPresenca(propostaIdInvalido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, new(null, null, null, null, null, null, null), null));

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _validadorDominioMock
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
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, new(null, null, null, null, null, null, null), null));

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _validadorDominioMock
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
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
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
                i.DataRetificacao == codafRetificacoes[0].DataRetificacao &&
                i.PaginaRetificacaoDom == codafRetificacoes[0].PaginaRetificacaoDom
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
            var codafRetificacao = codafRetificacoes[0];
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
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
                .Returns(codafRetificacao);
            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterPorListaPresencaIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafRetificacoesExistentes);

            // Act
            await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            _validatorMock
                .Verify(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default), Times.Once);
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Atualizar(It.Is<CodafRetificacaoListaPresenca>(i =>
                i.Id == codafRetificacao.Id &&
                i.DataRetificacao == codafRetificacao.DataRetificacao &&
                i.PaginaRetificacaoDom == codafRetificacao.PaginaRetificacaoDom
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
            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
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
                id == codafRetificacoesExistentes[0]
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
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null));
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

        [Fact]
        public async Task DadoPerfilRestrito_QuandoTentarEditarListaDeOutroUsuario_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var loginCriadoLista = _faker.Internet.UserName();
            var loginUsuarioAtual = _faker.Internet.UserName();

            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, long.MaxValue),
                CriadoLogin = loginCriadoLista
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginUsuarioAtual);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.PARECERISTA);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Você não tem permissão para editar esta lista de presença.");
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoListaComSituacaoFinalizado_QuandoTentarEditar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var loginCriadoLista = _faker.Internet.UserName();

            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, long.MaxValue),
                CriadoLogin = loginCriadoLista
            };
            codafListaPresencaExistente.Iniciar();
            codafListaPresencaExistente.MarcarComoEnviadaParaDf();
            codafListaPresencaExistente.Finalizar();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginCriadoLista);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _validadorDominioMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _validadorDominioMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível editar uma lista de presença com situação 'Finalizado'.");
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoEdicaoComAnexos_QuandoExecutar_EntaoDeveProcessarAnexosCorretamente()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var loginCriadoLista = _faker.Internet.UserName();

            var anexosDto = new List<CodafAnexoSalvarDto>
            {
                new() {
                    ArquivoCodigo = Guid.NewGuid(),
                    NomeArquivo = _faker.System.FileName(),
                    TipoAnexoId = TipoAnexoCodaf.ListaPresenca
                }
            };

            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Anexos = anexosDto
            };

            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, long.MaxValue),
                CriadoLogin = loginCriadoLista
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginCriadoLista);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _validadorDominioMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _validadorDominioMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _mapperMock
                .Setup(m => m.Map<IEnumerable<CodafAnexo>>(anexosDto))
                .Returns(anexosDto.Select(a => new CodafAnexo 
                {
                    ArquivoCodigo = a.ArquivoCodigo,
                    NomeArquivo = a.NomeArquivo,
                    Extensao = System.IO.Path.GetExtension(a.NomeArquivo),
                    TipoAnexoId = a.TipoAnexoId
                }));

            // Act
            await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

            // Assert
            _anexosServiceMock.Verify(
                a => a.ProcessarAnexosAsync(
                    It.Is<long>(id => id == codafListaPresencaExistente.Id),
                    It.IsAny<IEnumerable<CodafAnexo>>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoEdicaoComSucesso_QuandoExecutar_EntaoDeveRegistrarMovimentacao()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var loginCriadoLista = _faker.Internet.UserName();

            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, long.MaxValue),
                CriadoLogin = loginCriadoLista
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginCriadoLista);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _validadorDominioMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _validadorDominioMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _movimentacaoServiceMock.Verify(
                m => m.RegistrarMovimentacaoAsync(
                    It.Is<CodafListaPresenca>(l => l.Id == codafListaPresencaExistente.Id)),
                Times.Once);
        }

        [Fact]
        public async Task DadoEdicaoComInscritos_QuandoExecutar_EntaoDeveSalvarInscritosCorretamente()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var loginCriadoLista = _faker.Internet.UserName();

            var inscritos = new List<CodafInscritoListaPresencaSalvarDto>
            {
                new() {
                    InscricaoId = _faker.Random.Long(1, long.MaxValue),
                    PercentualFrequencia = 100
                }
            };

            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Inscritos = inscritos
            };

            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, long.MaxValue),
                CriadoLogin = loginCriadoLista
            };

            var inscrectosEntidades = new List<CodafInscricaoListaPresenca>
            {
                new() {
                    InscricaoId = inscritos[0].InscricaoId,
                    PercentualFrequencia = inscritos[0].PercentualFrequencia
                }
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginCriadoLista);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _validadorDominioMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _validadorDominioMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _mapperMock
                .Setup(m => m.Map<List<CodafInscricaoListaPresenca>>(inscritos))
                .Returns(inscrectosEntidades);

            // Act
            await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

            // Assert
            _inscritosServiceMock.Verify(
                i => i.SalvarInscritosAsync(
                    It.Is<List<CodafInscricaoListaPresenca>>(l => l.Count == inscritos.Count),
                    It.Is<long>(id => id == codafListaPresencaExistente.Id)),
                Times.Once);
        }

        [Fact]
        public async Task DadoTransacaoComFalhaAoSalvarInscritos_QuandoExecutar_EntaoDeveExecutarRollback()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var loginCriadoLista = _faker.Internet.UserName();

            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Inscritos =
                [
                    new() {
                        InscricaoId = _faker.Random.Long(1, long.MaxValue)
                    }
                ]
            };

            var codafListaPresencaExistente = new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, long.MaxValue),
                CriadoLogin = loginCriadoLista
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListaPresencaExistente);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginCriadoLista);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaEdicaoDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _validadorDominioMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _validadorDominioMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((Erro?)null);
            _mapperMock
                .Setup(m => m.Map<List<CodafInscricaoListaPresenca>>(It.IsAny<IEnumerable<CodafInscritoListaPresencaSalvarDto>>()))
                .Returns([new() { InscricaoId = _faker.Random.Long(1, long.MaxValue) }]);
            _inscritosServiceMock
                .Setup(i => i.SalvarInscritosAsync(It.IsAny<List<CodafInscricaoListaPresenca>>(), It.IsAny<long>()))
                .ThrowsAsync(new Exception("Erro ao salvar inscritos"));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}