using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Servicos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos.Notificacao
{
    public class GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandler _sut;
        private readonly Faker _faker;

        public GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");
            _sut = _mocker.CreateInstance<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandler>();
        }

        [Fact]
        public async Task DadoPropostaAprovada_QuandoGerar_EntaoDeveCriarNotificacaoComMensagemDeAprovacao()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var movimentacao = CriarMovimentacao(SituacaoProposta.Aprovada);
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            ConfigurarMocks(proposta.Id, areaPromotora, movimentacao, usuario, proposta.CriadoLogin);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(
                    It.Is<Dominio.Entidades.Notificacao>(n =>
                        n.Titulo.Contains(proposta.Id.ToString()) &&
                        n.Titulo.Contains(proposta.NomeFormacao) &&
                        n.Mensagem.Contains("aprovada") &&
                        n.Categoria == NotificacaoCategoria.Aviso &&
                        n.Tipo == NotificacaoTipo.Proposta &&
                        n.TipoEnvio == NotificacaoTipoEnvio.Email),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoPropostaRecusada_QuandoGerar_EntaoDeveCriarNotificacaoComMensagemDeRecusa()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var movimentacao = CriarMovimentacao(SituacaoProposta.Recusada);
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            ConfigurarMocks(proposta.Id, areaPromotora, movimentacao, usuario, proposta.CriadoLogin);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(
                    It.Is<Dominio.Entidades.Notificacao>(n =>
                        n.Mensagem.Contains("recusada")),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoMovimentacaoComJustificativa_QuandoGerar_EntaoDeveIncluirMotivoNaMensagem()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var justificativa = _faker.Lorem.Sentence();
            var movimentacao = CriarMovimentacao(SituacaoProposta.Recusada, justificativa);
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            ConfigurarMocks(proposta.Id, areaPromotora, movimentacao, usuario, proposta.CriadoLogin);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(
                    It.Is<Dominio.Entidades.Notificacao>(n =>
                        n.Mensagem.Contains("Motivo:") &&
                        n.Mensagem.Contains(justificativa)),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoMovimentacaoSemJustificativa_QuandoGerar_EntaoNaoDeveIncluirMotivoNaMensagem()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var movimentacao = CriarMovimentacao(SituacaoProposta.Aprovada, null);
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            ConfigurarMocks(proposta.Id, areaPromotora, movimentacao, usuario, proposta.CriadoLogin);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(
                    It.Is<Dominio.Entidades.Notificacao>(n =>
                        !n.Mensagem.Contains("Motivo:")),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoMovimentacaoNaoEncontrada_QuandoGerar_EntaoDeveLancarExcecao()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            _mocker.GetMock<IRepositorioAreaPromotora>()
                .Setup(x => x.ObterAreaPromotoraPorPropostaId(proposta.Id))
                .ReturnsAsync(areaPromotora);

            _mocker.GetMock<IRepositorioPropostaMovimentacao>()
                .Setup(x => x.ObterPorPropostaId(proposta.Id))
                .ReturnsAsync((PropostaMovimentacao)null!);

            // Act
            Func<Task> acao = async () => await _sut.Handle(command, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<Exception>()
                .WithMessage(MensagemNegocio.MOVIMENTACAO_PROPOSTA_NAO_ENCONTRADA);

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(It.IsAny<Dominio.Entidades.Notificacao>(), It.IsAny<CancellationToken>()), 
                    Times.Never);
        }

        [Fact]
        public async Task DadoProposta_QuandoGerar_EntaoDeveEnviarParaAreaPromotoraECriadorProposta()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var movimentacao = CriarMovimentacao(SituacaoProposta.Aprovada);
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            ConfigurarMocks(proposta.Id, areaPromotora, movimentacao, usuario, proposta.CriadoLogin);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(
                    It.Is<Dominio.Entidades.Notificacao>(n => n.Usuarios.Count() == 2),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoProposta_QuandoGerar_EntaoDeveIncluirParametrosComPropostaId()
        {
            // Arrange
            var proposta = CriarProposta();
            var areaPromotora = CriarAreaPromotora();
            var movimentacao = CriarMovimentacao(SituacaoProposta.Aprovada);
            var usuario = CriarUsuario();

            var command = new GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(proposta);

            ConfigurarMocks(proposta.Id, areaPromotora, movimentacao, usuario, proposta.CriadoLogin);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IServicoNotificacao>()
                .Verify(x => x.PersistirEEnviarAsync(
                    It.Is<Dominio.Entidades.Notificacao>(n =>
                        n.Parametros.Contains(proposta.Id.ToString())),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        private Proposta CriarProposta()
        {
            return new Proposta
            {
                Id = _faker.Random.Long(1, 10000),
                NomeFormacao = _faker.Lorem.Sentence(),
                CriadoLogin = _faker.Internet.UserName(),
                CriadoPor = _faker.Person.FullName
            };
        }

        private AreaPromotora CriarAreaPromotora()
        {
            return new AreaPromotora
            {
                Id = _faker.Random.Long(1, 100),
                Nome = _faker.Company.CompanyName(),
                Email = _faker.Internet.Email()
            };
        }

        private PropostaMovimentacao CriarMovimentacao(SituacaoProposta situacao, string? justificativa = null)
        {
            return new PropostaMovimentacao
            {
                Id = _faker.Random.Long(1, 1000),
                Situacao = situacao,
                Justificativa = justificativa
            };
        }

        private Usuario CriarUsuario()
        {
            return new Usuario
            {
                Id = _faker.Random.Long(1, 1000),
                Login = _faker.Internet.UserName(),
                Nome = _faker.Person.FullName,
                Email = _faker.Internet.Email()
            };
        }

        private void ConfigurarMocks(long propostaId, AreaPromotora areaPromotora, 
            PropostaMovimentacao movimentacao, Usuario usuario, string loginCriador)
        {
            _mocker.GetMock<IRepositorioAreaPromotora>()
                .Setup(x => x.ObterAreaPromotoraPorPropostaId(propostaId))
                .ReturnsAsync(areaPromotora);

            _mocker.GetMock<IRepositorioPropostaMovimentacao>()
                .Setup(x => x.ObterPorPropostaId(propostaId))
                .ReturnsAsync(movimentacao);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(x => x.ObterPorLogin(loginCriador))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<IEnumerable<NotificacaoUsuario>>(It.IsAny<IEnumerable<NotificacaoUsuario>>()))
                .Returns((IEnumerable<NotificacaoUsuario> destinatarios) => destinatarios);

            _mocker.GetMock<IServicoNotificacao>()
                .Setup(x => x.PersistirEEnviarAsync(It.IsAny<Dominio.Entidades.Notificacao>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }
    }
}
