using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaParecerPorPropostaIdECampoQueryHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaPareceristaConsideracao> _repositorioPropostaPareceristaConsideracao;
        private readonly Mock<IRepositorioProposta> _repositorioProposta;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IMediator> _mediator;
        private readonly ObterPropostaParecerPorPropostaIdECampoQueryHandler _sut;

        public ObterPropostaParecerPorPropostaIdECampoQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioPropostaPareceristaConsideracao = mocker.GetMock<IRepositorioPropostaPareceristaConsideracao>();
            _repositorioProposta = mocker.GetMock<IRepositorioProposta>();
            _mapper = mocker.GetMock<IMapper>();
            _mediator = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<ObterPropostaParecerPorPropostaIdECampoQueryHandler>();
        }

        [Fact]
        public void DadoRepositorioConsideracaoNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IRepositorioPropostaPareceristaConsideracao repoNulo = null!;

            // Act
            var act = () => new ObterPropostaParecerPorPropostaIdECampoQueryHandler(repoNulo, _mapper.Object, _mediator.Object, _repositorioProposta.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("repositorioPropostaPareceristaConsideracao");
        }

        [Fact]
        public void DadoRepositorioPropostaNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IRepositorioProposta repoNulo = null!;

            // Act
            var act = () => new ObterPropostaParecerPorPropostaIdECampoQueryHandler(_repositorioPropostaPareceristaConsideracao.Object, _mapper.Object, _mediator.Object, repoNulo);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("repositorioProposta");
        }

        [Fact]
        public void DadoMapperNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IMapper mapperNulo = null!;

            // Act
            var act = () => new ObterPropostaParecerPorPropostaIdECampoQueryHandler(_repositorioPropostaPareceristaConsideracao.Object, mapperNulo, _mediator.Object, _repositorioProposta.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("mapper");
        }

        [Fact]
        public void DadoMediatorNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IMediator mediatorNulo = null!;

            // Act
            var act = () => new ObterPropostaParecerPorPropostaIdECampoQueryHandler(_repositorioPropostaPareceristaConsideracao.Object, _mapper.Object, mediatorNulo, _repositorioProposta.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("mediator");
        }

        [Fact]
        public async Task DadoNenhumaConsideracaoEUsuarioPareceristaElegivel_QuandoProcessarQuery_EntaoDeveRetornarVazioComPodeInserirTrue()
        {
            // Arrange
            var query = CriarQueryValida();
            var proposta = CriarProposta(SituacaoProposta.AguardandoAnalisePeloParecerista);
            var usuario = CriarUsuario("1234567");
            var parecerista = CriarPropostaParecerista("1234567", SituacaoParecerista.AguardandoValidacao);

            ConfigurarDependenciasBasicas(query, proposta, usuario, Perfis.PARECERISTA, [parecerista], []);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PropostaId.Should().Be(proposta.Id);
            resultado.PodeInserir.Should().BeTrue();
            resultado.Itens.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoNenhumaConsideracaoEUsuarioAdminDF_QuandoProcessarQuery_EntaoDeveRetornarVazioComPodeInserirFalse()
        {
            // Arrange
            var query = CriarQueryValida();
            var proposta = CriarProposta(SituacaoProposta.AguardandoAnalisePeloParecerista);
            var usuario = CriarUsuario("admin_login");

            ConfigurarDependenciasBasicas(query, proposta, usuario, Perfis.ADMIN_DF, [], []);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PodeInserir.Should().BeFalse();
            resultado.Itens.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoConsideracoesEUsuarioParecerista_QuandoProcessarQuery_EntaoDeveMapearItensEValidarPermissoes()
        {
            // Arrange
            var query = CriarQueryValida();
            var proposta = CriarProposta(SituacaoProposta.AguardandoAnalisePeloParecerista);
            var usuario = CriarUsuario("1234567");
            var parecerista = CriarPropostaParecerista("1234567", SituacaoParecerista.AguardandoValidacao);
            var consideracao = CriarConsideracao("1234567");
            var consideracoesDTO = CriarListaConsideracoesDTO();

            ConfigurarDependenciasBasicas(query, proposta, usuario, Perfis.PARECERISTA, [parecerista], [consideracao]);
            ConfigurarMapeamentoParecerista(consideracoesDTO);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PodeInserir.Should().BeFalse();
            resultado.Itens.Should().HaveCount(1);
            resultado.Itens.First().PodeAlterar.Should().BeTrue();
        }

        [Fact]
        public async Task DadoConsideracoesEUsuarioAdminDF_QuandoProcessarQuery_EntaoDeveRetornarApenasEnviadasEAguardandoRevalidacao()
        {
            // Arrange
            var query = CriarQueryValida();
            var proposta = CriarProposta(SituacaoProposta.AguardandoAnaliseParecerPelaDF);
            var usuario = CriarUsuario("admin_login");

            var pareceristaEnviado = CriarPropostaParecerista("7654321", SituacaoParecerista.Enviada);
            var consideracao = CriarConsideracao("7654321");
            consideracao.PropostaPareceristaId = pareceristaEnviado.Id;
            var consideracoesDTO = CriarListaConsideracoesDTO();

            ConfigurarDependenciasBasicas(query, proposta, usuario, Perfis.ADMIN_DF, [pareceristaEnviado], [consideracao]);

            _mapper.Setup(m => m.Map<IEnumerable<PropostaPareceristaConsideracaoDTO>>(It.IsAny<IEnumerable<PropostaPareceristaConsideracao>>()))
                   .Returns((IEnumerable<PropostaPareceristaConsideracao> source) =>
                       source.Any() ? consideracoesDTO : Enumerable.Empty<PropostaPareceristaConsideracaoDTO>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PodeInserir.Should().BeFalse();
            resultado.Itens.Should().HaveCount(1);
            resultado.Itens.First().PodeAlterar.Should().BeTrue();
        }

        [Fact]
        public async Task DadoConsideracoesEUsuarioAreaPromotora_QuandoProcessarQuery_EntaoDeveLimparAuditoriaERetornarPodeAlterarFalse()
        {
            // Arrange
            var query = CriarQueryValida();
            var proposta = CriarProposta(SituacaoProposta.AnaliseParecerPelaAreaPromotora);
            var usuario = CriarUsuario("ap_login");
            var guidAreaPromotora = Guid.NewGuid();

            var pareceristaEnviado = CriarPropostaParecerista("999999", SituacaoParecerista.Enviada);
            var consideracao = CriarConsideracao("999999");
            consideracao.PropostaPareceristaId = pareceristaEnviado.Id;
            var consideracoesDTO = CriarListaConsideracoesDTO();

            ConfigurarDependenciasBasicas(query, proposta, usuario, guidAreaPromotora, [pareceristaEnviado], [consideracao]);

            _mediator.Setup(m => m.Send(It.IsAny<ObterPerfilAreaPromotoraQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new RetornoListagemDTO { Id = 1, Descricao = "Área Promotora" });

            _mapper.Setup(m => m.Map<IEnumerable<PropostaPareceristaConsideracaoDTO>>(It.IsAny<IEnumerable<PropostaPareceristaConsideracao>>()))
                   .Returns((IEnumerable<PropostaPareceristaConsideracao> source) =>
                       source.Any() ? consideracoesDTO
                                    : []);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PodeInserir.Should().BeFalse();
            resultado.Itens.Should().HaveCount(1);

            var itemRetornado = resultado.Itens.First();
            itemRetornado.Auditoria.Should().BeNull();
            itemRetornado.PodeAlterar.Should().BeFalse();
        }

        #region Factory Methods

        private static ObterPropostaParecerPorPropostaIdECampoQuery CriarQueryValida()
        {
            return new ObterPropostaParecerPorPropostaIdECampoQuery(1, CampoConsideracao.FormacaoHomologada);
        }

        private void ConfigurarDependenciasBasicas(
            ObterPropostaParecerPorPropostaIdECampoQuery query,
            Proposta proposta,
            Usuario usuario,
            Guid perfilLogado,
            IEnumerable<PropostaParecerista> pareceristas,
            IEnumerable<PropostaPareceristaConsideracao> consideracoes)
        {
            _repositorioPropostaPareceristaConsideracao
                .Setup(r => r.ObterPorPropostaIdECampo(query.PropostaId, query.CampoConsideracao))
                .ReturnsAsync(consideracoes);

            _mediator.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(perfilLogado);

            _mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(usuario);

            _repositorioProposta.Setup(r => r.ObterPorId(query.PropostaId))
                                .ReturnsAsync(proposta);

            _mediator.Setup(m => m.Send(It.IsAny<ObterPareceristasAdicionadosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(pareceristas);

            _mediator.Setup(m => m.Send(It.IsAny<ObterPerfilAreaPromotoraQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((RetornoListagemDTO)null!);
        }

        private void ConfigurarMapeamentoParecerista(IEnumerable<PropostaPareceristaConsideracaoDTO> dto)
        {
            _mapper.Setup(m => m.Map<IEnumerable<PropostaPareceristaConsideracaoDTO>>(It.IsAny<IEnumerable<PropostaPareceristaConsideracao>>()))
                   .Returns(dto);
        }

        private static Proposta CriarProposta(SituacaoProposta situacao)
        {
            return new Proposta { Id = 1, Situacao = situacao };
        }

        private static Usuario CriarUsuario(string login)
        {
            return new Usuario { Login = login };
        }

        private static PropostaParecerista CriarPropostaParecerista(string registroFuncional, SituacaoParecerista situacao)
        {
            return new PropostaParecerista
            {
                Id = 99,
                RegistroFuncional = registroFuncional,
                Situacao = situacao
            };
        }

        private static PropostaPareceristaConsideracao CriarConsideracao(string loginCriador)
        {
            return new PropostaPareceristaConsideracao
            {
                Id = 1,
                CriadoLogin = loginCriador,
                AlteradoEm = DateTime.Now
            };
        }

        private static List<PropostaPareceristaConsideracaoDTO> CriarListaConsideracoesDTO()
        {
            return [new PropostaPareceristaConsideracaoDTO { Id = 1, Auditoria = new AuditoriaDTO() }];
        }

        #endregion
    }
}
