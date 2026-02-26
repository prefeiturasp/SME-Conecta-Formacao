using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class AlterarPropostaCommandHandlerTestes
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ITransacao> _transacao;
        private readonly Mock<IRepositorioProposta> _repositorioProposta;
        private readonly AlterarPropostaCommandHandler _sut;

        public AlterarPropostaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _mediator = mocker.GetMock<IMediator>();
            _mapper = mocker.GetMock<IMapper>();
            _transacao = mocker.GetMock<ITransacao>();
            _repositorioProposta = mocker.GetMock<IRepositorioProposta>();

            _sut = mocker.CreateInstance<AlterarPropostaCommandHandler>();
        }

        [Fact]
        public void DadoMediatorNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IMediator mediatorNulo = null!;

            // Act
            var act = () => new AlterarPropostaCommandHandler(mediatorNulo, _mapper.Object, _transacao.Object, _repositorioProposta.Object);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void DadoMapperNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IMapper mapperNulo = null!;

            // Act
            var act = () => new AlterarPropostaCommandHandler(_mediator.Object, mapperNulo, _transacao.Object, _repositorioProposta.Object);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void DadoTransacaoNula_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            ITransacao transacaoNula = null!;

            // Act
            var act = () => new AlterarPropostaCommandHandler(_mediator.Object, _mapper.Object, transacaoNula, _repositorioProposta.Object);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void DadoRepositorioPropostaNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IRepositorioProposta repositorioPropostaNulo = null!;

            // Act
            var act = () => new AlterarPropostaCommandHandler(_mediator.Object, _mapper.Object, _transacao.Object, repositorioPropostaNulo);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
        [Fact]
        public async Task DadoPropostaInexistente_QuandoProcessarComando_EntaoDeveLancarNegocioExceptionNotFound()
        {
            // Arrange
            var comando = new AlterarPropostaCommand(1, CriarPropostaDtoValida());

            _repositorioProposta
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync((Proposta)null!);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.StatusCode.Should().Be((int)System.Net.HttpStatusCode.NotFound);
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaComPublicoEFuncaoOutrosPreenchidosSemItens_QuandoProcessarComando_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaDto = CriarPropostaDtoValida();
            propostaDto.PublicoAlvoOutros = "Outros públicos";
            propostaDto.FuncaoEspecificaOutros = "Outras funções";
            propostaDto.PublicosAlvo = [];
            propostaDto.FuncoesEspecificas = [];
            propostaDto.ComponentesCurriculares = [];
            propostaDto.AnosTurmas = [];

            var comando = new AlterarPropostaCommand(1, propostaDto);
            var proposta = CriarPropostaValida();

            _repositorioProposta
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(proposta);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarPublicoAlvoOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarFuncaoEspecificaOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarCriterioValidacaoInscricaoOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.INFORMAR_PUBLICO_FUNCAO_MODALIDADE);
        }

        [Fact]
        public async Task DadoValidacaoPublicoAlvoOutrosComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = new AlterarPropostaCommand(1, CriarPropostaDtoValida());
            var mensagemErro = "Erro na validação do público alvo outros";

            ConfigurarDependenciasIniciaisComSucesso();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarPublicoAlvoOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoFuncaoEspecificaOutrosComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = new AlterarPropostaCommand(1, CriarPropostaDtoValida());
            var mensagemErro = "Erro na validação de função específica outros";

            ConfigurarDependenciasIniciaisComSucesso();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarFuncaoEspecificaOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoCriterioInscricaoOutrosComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = new AlterarPropostaCommand(1, CriarPropostaDtoValida());
            var mensagemErro = "Erro na validação de critério de inscrição outros";

            ConfigurarDependenciasIniciaisComSucesso();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarCriterioValidacaoInscricaoOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoModalidadeAnoTurmaComponenteComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = new AlterarPropostaCommand(1, CriarPropostaDtoValida());
            var mensagemErro = "Erro na validação de modalidades, turmas e componentes";

            ConfigurarDependenciasIniciaisComSucesso();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoTurmaComArrayDreIdsVazio_QuandoProcessarComando_EntaoDeveAcumularErroELancarNegocioException()
        {
            // Arrange
            var propostaDto = CriarPropostaDtoValida();
            propostaDto.Turmas = [new PropostaTurmaDTO { DresIds = [] }];

            var comando = new AlterarPropostaCommand(1, propostaDto);

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.DRE_NAO_INFORMADA_PARA_TODAS_AS_TURMAS);
        }

        [Fact]
        public async Task DadoPropostaSemTurmas_QuandoProcessarComando_EntaoDeveAcumularErroELancarNegocioException()
        {
            // Arrange
            var propostaDto = CriarPropostaDtoValida();
            propostaDto.Turmas = [];

            var comando = new AlterarPropostaCommand(1, propostaDto);

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.TODAS_AS_TURMAS_DEVEM_POSSUIR_DRE_OU_OPCAO_TODOS);
        }

        [Fact]
        public async Task DadoValidacaoDataInscricaoComErro_QuandoProcessarComando_EntaoDeveAcumularErroELancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var mensagemErro = "Data de inscrição inválida";

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mensagemErro);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoRegenteTurmaComErro_QuandoProcessarComando_EntaoDeveAcumularErroELancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var mensagemErro = "Erro na validação do regente da turma";

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarSeExisteRegenteTurmaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mensagemErro);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoInformacoesGeraisComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var mensagemErro = "Erro nas informações gerais";

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarInformacoesGeraisCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoDatasExistentesComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var mensagemErro = "Erro nas datas da proposta";

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarDatasExistentesNaPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoDetalhamentoComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var mensagemErro = "Erro no detalhamento da proposta";

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarDetalhamentoDaPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoValidacaoCertificacaoComErro_QuandoProcessarComando_EntaoDeveAcumularFalhaELancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var mensagemErro = "Erro na certificação da proposta";

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarCertificacaoPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagemErro]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();

            excecao.Which.Mensagens.Should().Contain(mensagemErro);
        }

        [Fact]
        public async Task DadoPropostaNaoPublicada_QuandoProcessarComandoComSucesso_EntaoDeveRealizarCommitERetornarMensagemPadrao()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _repositorioProposta
                .Setup(r => r.Atualizar(It.IsAny<Proposta>()))
                .ReturnsAsync(CriarPropostaValida()); // Retorna sucesso na atualização

            _mediator
                .Setup(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(string.Format(MensagemNegocio.PROPOSTA_X_ALTERADA_COM_SUCESSO, comando.Id));

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
            transacaoDbMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaPublicadaComInscricaoManual_QuandoProcessarComandoComSucesso_EntaoDeveRealizarCommitERetornarMensagemPadrao()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            comando.PropostaDTO.TiposInscricao = [new PropostaTipoInscricaoDTO { TipoInscricao = Dominio.Enumerados.TipoInscricao.Manual }];

            var propostaPublicada = CriarPropostaValida();
            propostaPublicada.Situacao = Dominio.Enumerados.SituacaoProposta.Publicada;

            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            ConfigurarDependenciasIniciaisComSucesso();
            _repositorioProposta.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(propostaPublicada); // Sobrescreve para Publicada
            ConfigurarMapeamentoEComandosIntermediarios();

            _repositorioProposta.Setup(r => r.Atualizar(It.IsAny<Proposta>())).ReturnsAsync(propostaPublicada);
            _mediator.Setup(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(string.Format(MensagemNegocio.PROPOSTA_X_ALTERADA_COM_SUCESSO, comando.Id));

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaPublicadaComInscricaoAutomatica_QuandoProcessarComandoComSucesso_EntaoDeveRealizarCommitEAnexarMensagemDeAviso()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            comando.PropostaDTO.TiposInscricao = [new PropostaTipoInscricaoDTO { TipoInscricao = Dominio.Enumerados.TipoInscricao.Automatica }];

            var propostaPublicada = CriarPropostaValida();
            propostaPublicada.Situacao = Dominio.Enumerados.SituacaoProposta.Publicada;

            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            ConfigurarDependenciasIniciaisComSucesso();
            _repositorioProposta.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(propostaPublicada); // Sobrescreve para Publicada
            ConfigurarMapeamentoEComandosIntermediarios();

            _repositorioProposta.Setup(r => r.Atualizar(It.IsAny<Proposta>())).ReturnsAsync(propostaPublicada);
            _mediator.Setup(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var mensagemEsperada = string.Format(MensagemNegocio.PROPOSTA_X_ALTERADA_COM_SUCESSO, comando.Id) + MensagemNegocio.PROPOSTA_PUBLICADA_ALTERADA_COM_INSCRICAO_AUTOMATICA;

            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(mensagemEsperada);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoErroNaPersistencia_QuandoProcessarComando_EntaoDeveRealizarRollbackEPropagarExcecao()
        {
            // Arrange
            var comando = CriarComandoValidoParaValidacoesFinais();
            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            ConfigurarDependenciasIniciaisComSucesso();
            ConfigurarMapeamentoEComandosIntermediarios();

            _repositorioProposta
                .Setup(r => r.Atualizar(It.IsAny<Proposta>()))
                .ThrowsAsync(new Exception("Erro genérico de banco de dados"));

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro genérico de banco de dados");

            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Never);
        }

        #region Factory Methods

        private Mock<System.Data.IDbTransaction> ConfigurarTransacaoComSucesso()
        {
            var transacaoDbMock = new Mock<System.Data.IDbTransaction>();

            _transacao
                .Setup(t => t.Iniciar())
                .Returns(transacaoDbMock.Object);

            return transacaoDbMock;
        }

        private AlterarPropostaCommand CriarComandoValidoParaValidacoesFinais()
        {
            var propostaDto = CriarPropostaDtoValida();

            // Garante que passa pela validação de turmas e DREs usando o Id 99 (opção "TODAS" configurada no mock)
            propostaDto.Turmas = [new PropostaTurmaDTO { DresIds = [99] }];

            return new AlterarPropostaCommand(1, propostaDto);
        }

        private void ConfigurarMapeamentoEComandosIntermediarios()
        {
            _mapper
                .Setup(m => m.Map<Proposta>(It.IsAny<PropostaDTO>()))
                .Returns(CriarPropostaValida());

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarResponsavelDfCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarAreaPromotoraCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mediator
                .Setup(m => m.Send(It.IsAny<ObterDreTodosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dominio.Entidades.Dre { Id = 99, Nome = "TODAS" });

            // Configurações padrão para as validações finais retornarem sucesso (vazio)
            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarSeExisteRegenteTurmaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarInformacoesGeraisCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarDatasExistentesNaPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarDetalhamentoDaPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarCertificacaoPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);
        }

        private void ConfigurarDependenciasIniciaisComSucesso()
        {
            _repositorioProposta
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(CriarPropostaValida());

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarPublicoAlvoOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarFuncaoEspecificaOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarCriterioValidacaoInscricaoOutrosCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediator
                .Setup(m => m.Send(It.IsAny<ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);
        }

        private PropostaDTO CriarPropostaDtoValida()
        {
            return new PropostaDTO
            {
                NomeFormacao = "Formação Teste",
                TiposInscricao = []
            };
        }

        private Proposta CriarPropostaValida()
        {
            return new Proposta
            {
                Id = 1,
                Situacao = Dominio.Enumerados.SituacaoProposta.Cadastrada
            };
        }

        #endregion
    }
}
