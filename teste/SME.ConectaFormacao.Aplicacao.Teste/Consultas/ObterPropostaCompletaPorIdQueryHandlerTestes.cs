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
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaCompletaPorIdQueryHandlerTestes
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IRepositorioProposta> _repositorioProposta;
        private readonly Mock<IRepositorioArquivo> _repositorioArquivo;
        private readonly Mock<IRepositorioPropostaMovimentacao> _repositorioPropostaMovimentacao;
        private readonly Mock<IRepositorioAreaPromotora> _repositorioAreaPromotora;
        private readonly Mock<IMediator> _mediator;
        private readonly ObterPropostaCompletaPorIdQueryHandler _sut;

        public ObterPropostaCompletaPorIdQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _mapper = mocker.GetMock<IMapper>();
            _repositorioProposta = mocker.GetMock<IRepositorioProposta>();
            _repositorioArquivo = mocker.GetMock<IRepositorioArquivo>();
            _repositorioPropostaMovimentacao = mocker.GetMock<IRepositorioPropostaMovimentacao>();
            _repositorioAreaPromotora = mocker.GetMock<IRepositorioAreaPromotora>();
            _mediator = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<ObterPropostaCompletaPorIdQueryHandler>();
        }

        [Fact]
        public async Task DadoPropostaInexistente_QuandoProcessarQuery_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);

            _repositorioProposta
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync((Proposta)null!);

            // Act
            var act = async () => await _sut.Handle(query, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaExcluida_QuandoProcessarQuery_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var propostaExcluida = CriarPropostaExcluida();

            _repositorioProposta
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(propostaExcluida);

            // Act
            var act = async () => await _sut.Handle(query, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaComTurmas_QuandoProcessarQuery_EntaoDeveBuscarDresParaCadaTurma()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Turmas = [new PropostaTurma { Id = 10 }, new PropostaTurma { Id = 20 }];

            ConfigurarDependenciasComunsParaSucesso(proposta);

            // Act
            await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioProposta.Verify(r => r.ObterPropostaTurmasDresPorPropostaTurmaId(10), Times.Once);
            _repositorioProposta.Verify(r => r.ObterPropostaTurmasDresPorPropostaTurmaId(20), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaComArquivoImagem_QuandoProcessarQuery_EntaoDeveBuscarArquivoEMapearDTO()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.ArquivoImagemDivulgacaoId = 99;

            ConfigurarDependenciasComunsParaSucesso(proposta);

            var arquivo = new Arquivo { Id = 99 };
            _repositorioArquivo.Setup(r => r.ObterPorId(99)).ReturnsAsync(arquivo);

            var imagemDto = new PropostaImagemDivulgacaoDTO { ArquivoId = 99 };
            _mapper.Setup(m => m.Map<PropostaImagemDivulgacaoDTO>(arquivo)).Returns(imagemDto);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioArquivo.Verify(r => r.ObterPorId(99), Times.Once);
            _mapper.Verify(m => m.Map<PropostaImagemDivulgacaoDTO>(arquivo), Times.Once);
            resultado.ArquivoImagemDivulgacao.Should().BeEquivalentTo(imagemDto);
        }

        [Fact]
        public async Task DadoPropostaSemArquivoImagem_QuandoProcessarQuery_EntaoNaoDeveBuscarArquivo()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.ArquivoImagemDivulgacaoId = null;

            ConfigurarDependenciasComunsParaSucesso(proposta);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioArquivo.Verify(r => r.ObterPorId(It.IsAny<long>()), Times.Never);
            resultado.ArquivoImagemDivulgacao.Should().BeNull();
        }

        [Fact]
        public async Task DadoUsuarioPareceristaDaProposta_QuandoProcessarQuery_EntaoDeveConfigurarLabelsSugerirAprovacaoERecusa()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            var parecerista = new PropostaParecerista { RegistroFuncional = "12345" };
            proposta.Pareceristas = [parecerista];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.PARECERISTA, "12345");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.EhParecerista.Should().BeTrue();
            resultado.LabelAprovar.Should().Be("Sugerir aprovação");
            resultado.LabelRecusar.Should().Be("Sugerir recusa");
        }

        [Fact]
        public async Task DadoPareceristaAvaliandoPropostaComSuaSituacaoAprovada_QuandoProcessarQuery_EntaoNaoPodeAprovarOuRecusarERetornaSuaJustificativa()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();

            var parecerista = new PropostaParecerista
            {
                RegistroFuncional = "12345",
                Situacao = SituacaoParecerista.Aprovada,
                Justificativa = "Justificativa do Parecerista"
            };
            proposta.Pareceristas = [parecerista];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.PARECERISTA, "12345");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.PodeAprovar.Should().BeFalse();
            resultado.PodeRecusar.Should().BeFalse();
            resultado.UltimaJustificativaAprovacaoRecusa.Should().Be("Justificativa do Parecerista");
        }

        [Fact]
        public async Task DadoPareceristaAvaliandoPropostaAguardandoAnaliseSemConsideracoes_QuandoProcessarQuery_EntaoPodeAprovarERecusar()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista;

            var parecerista = new PropostaParecerista
            {
                Id = 99,
                RegistroFuncional = "12345",
                Situacao = SituacaoParecerista.AguardandoValidacao
            };
            proposta.Pareceristas = [parecerista];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.PARECERISTA, "12345");

            _repositorioProposta
                .Setup(r => r.ObterPropostaPareceristaConsideracaoPorId(It.IsAny<long>()))
                .ReturnsAsync([]); // Sem considerações

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.PodeAprovar.Should().BeTrue();
            resultado.PodeRecusar.Should().BeTrue();
        }

        [Fact]
        public async Task DadoPareceristaAguardandoValidacaoComConsideracoesSalvas_QuandoProcessarQuery_EntaoPodeEnviarConsideracoes()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista;

            var parecerista = new PropostaParecerista
            {
                Id = 99,
                RegistroFuncional = "12345",
                Situacao = SituacaoParecerista.AguardandoValidacao
            };
            proposta.Pareceristas = [parecerista];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.PARECERISTA, "12345");

            var consideracoesSalvas = new List<PropostaPareceristaConsideracao>
            {
                new PropostaPareceristaConsideracao { PropostaPareceristaId = 99 }
            };

            _repositorioProposta
                .Setup(r => r.ObterPropostaPareceristaConsideracaoPorId(It.IsAny<long>()))
                .ReturnsAsync(consideracoesSalvas);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.PodeEnviarConsideracoes.Should().BeTrue();
            resultado.PodeAprovar.Should().BeFalse(); // Bloqueia aprovação se houver considerações
            resultado.PodeRecusar.Should().BeFalse();
        }

        [Fact]
        public async Task DadoUsuarioPareceristaNaoVinculadoAProposta_QuandoProcessarQuery_EntaoNaoPodeEnviarConsideracoesNemAprovar()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista;

            var pareceristaDiferente = new PropostaParecerista
            {
                RegistroFuncional = "OUTRO_USUARIO",
                Situacao = SituacaoParecerista.AguardandoValidacao
            };
            proposta.Pareceristas = [pareceristaDiferente];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.PARECERISTA, "12345"); // Usuário logado não está na lista

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.PodeEnviarConsideracoes.Should().BeFalse();
            resultado.PodeAprovar.Should().BeFalse();
            resultado.PodeRecusar.Should().BeFalse();
            resultado.UltimaJustificativaAprovacaoRecusa.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoUsuarioAdminDfAvaliandoPropostaAguardandoAnaliseFinal_QuandoProcessarQuery_EntaoPodeAprovarERecusar()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.AguardandoValidacaoFinalPelaDF;

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.ADMIN_DF, "admin_df");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.EhAdminDF.Should().BeTrue();
            resultado.PodeAprovar.Should().BeTrue();
            resultado.PodeRecusar.Should().BeTrue();
            resultado.LabelAprovar.Should().Be("Aprovar");
            resultado.LabelRecusar.Should().Be("Recusar");
        }

        [Fact]
        public async Task DadoUsuarioAdminDfAvaliandoPropostaAguardandoAnaliseDfComPareceristas_QuandoProcessarQuery_EntaoPodeEnviar()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.AguardandoAnaliseDf;
            proposta.Pareceristas = [new PropostaParecerista { Id = 1, RegistroFuncional = "parecerista_1" }];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.ADMIN_DF, "admin_df");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.EhAdminDF.Should().BeTrue();
            resultado.PodeEnviar.Should().BeTrue();
        }

        [Fact]
        public async Task DadoCalculoTotalDeConsideracoesParaAdminDF_QuandoProcessarQuery_EntaoDeveContabilizarApenasPareceristasEnviados()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();

            var pareceristaEnviado = new PropostaParecerista
            {
                Id = 10,
                Situacao = SituacaoParecerista.Enviada
            };
            var pareceristaAguardando = new PropostaParecerista
            {
                Id = 20,
                Situacao = SituacaoParecerista.AguardandoValidacao
            };

            proposta.Pareceristas = [pareceristaEnviado, pareceristaAguardando];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Perfis.ADMIN_DF, "admin_df");

            var consideracoes = new List<PropostaPareceristaConsideracao>
            {
                new PropostaPareceristaConsideracao { PropostaPareceristaId = 10, Campo = CampoConsideracao.NomeFormacao },
                new PropostaPareceristaConsideracao { PropostaPareceristaId = 10, Campo = CampoConsideracao.Justificativa },
                new PropostaPareceristaConsideracao { PropostaPareceristaId = 20, Campo = CampoConsideracao.Objetivos } // Não deve ser contabilizada
            };

            _repositorioProposta
                .Setup(r => r.ObterPropostaPareceristaConsideracaoPorId(It.IsAny<long>()))
                .ReturnsAsync(consideracoes);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.TotalDeConsideracoes.Should().HaveCount(2);
            resultado.TotalDeConsideracoes.Should().Contain(c => c.Campo == CampoConsideracao.NomeFormacao && c.Quantidade == 1);
            resultado.TotalDeConsideracoes.Should().Contain(c => c.Campo == CampoConsideracao.Justificativa && c.Quantidade == 1);
            resultado.TotalDeConsideracoes.Should().NotContain(c => c.Campo == CampoConsideracao.Objetivos);
        }
        [Fact]
        public async Task DadoUsuarioAreaPromotoraEPropostaDevolvida_QuandoProcessarQuery_EntaoDevePermitirEnvio()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.Devolvida;

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Guid.NewGuid(), "ap_usuario");

            // Configura o usuário logado como Área Promotora
            _mediator.Setup(m => m.Send(It.IsAny<ObterPerfilAreaPromotoraQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new RetornoListagemDTO { Id = 1, Descricao = "Área Promotora Teste" });

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.EhAreaPromotora.Should().BeTrue();
            resultado.PodeEnviar.Should().BeTrue();
        }

        [Fact]
        public async Task DadoUsuarioAreaPromotoraEPropostaEmAnaliseComPareceristas_QuandoProcessarQuery_EntaoDevePermitirEnvio()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.AnaliseParecerPelaAreaPromotora;
            proposta.Pareceristas = [new PropostaParecerista { Id = 1, RegistroFuncional = "parecerista_1" }];

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Guid.NewGuid(), "ap_usuario");

            // Configura o usuário logado como Área Promotora
            _mediator.Setup(m => m.Send(It.IsAny<ObterPerfilAreaPromotoraQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new RetornoListagemDTO { Id = 1, Descricao = "Área Promotora Teste" });

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.EhAreaPromotora.Should().BeTrue();
            resultado.PodeEnviar.Should().BeTrue();
        }

        [Fact]
        public async Task DadoPropostaComPublicoEFuncaoOutrosPreenchidos_QuandoProcessarQuery_EntaoDeveDesativarAnoEhComponente()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.PublicoAlvoOutros = "Público Outros Teste";
            proposta.FuncaoEspecificaOutros = "Função Outros Teste";

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Guid.NewGuid(), "usuario_comum");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.DesativarAnoEhComponente.Should().BeTrue();
        }

        [Fact]
        public async Task DadoPropostaSemPublicoNemFuncao_QuandoProcessarQuery_EntaoNaoDeveDesativarAnoEhComponente()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.PublicosAlvo = [];
            proposta.FuncoesEspecificas = [];
            proposta.PublicoAlvoOutros = string.Empty;
            proposta.FuncaoEspecificaOutros = string.Empty;

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Guid.NewGuid(), "usuario_comum");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.DesativarAnoEhComponente.Should().BeFalse();
        }

        [Fact]
        public async Task DadoPropostaSemPareceristas_QuandoProcessarQuery_EntaoNaoDeveExibirConsideracoes()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Pareceristas = []; // Lista vazia

            ConfigurarDependenciasComunsParaSucesso(proposta);
            // Configura como Admin DF, mas como não tem pareceristas, deve forçar ExibirConsideracoes = false
            ConfigurarPerfilEUsuarioLogado(Perfis.ADMIN_DF, "admin_df");

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.ExibirConsideracoes.Should().BeFalse();
        }

        [Fact]
        public async Task DadoPerfilComumEPropostaCadastrada_QuandoProcessarQuery_EntaoPodeEnviarEJustificativaVazia()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.Cadastrada;

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Guid.NewGuid(), "usuario_comum"); // Perfil comum (não é admin, nem parecerista, nem AP)

            // Retorna null para garantir que não é Área Promotora
            _mediator.Setup(m => m.Send(It.IsAny<ObterPerfilAreaPromotoraQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((RetornoListagemDTO)null!);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.PodeEnviar.Should().BeTrue();
            resultado.UltimaJustificativaAprovacaoRecusa.Should().BeEmpty();
            resultado.EhAdminDF.Should().BeFalse();
            resultado.EhParecerista.Should().BeFalse();
            resultado.EhAreaPromotora.Should().BeFalse();
        }

        [Fact]
        public async Task DadoPerfilComumEPropostaAprovada_QuandoProcessarQuery_EntaoDeveRetornarJustificativaDaMovimentacao()
        {
            // Arrange
            var query = new ObterPropostaCompletaPorIdQuery(1);
            var proposta = CriarPropostaValida();
            proposta.Situacao = SituacaoProposta.Aprovada;

            ConfigurarDependenciasComunsParaSucesso(proposta);
            ConfigurarPerfilEUsuarioLogado(Guid.NewGuid(), "usuario_comum");

            _repositorioPropostaMovimentacao
                .Setup(r => r.ObterUltimoParecerPropostaId(It.IsAny<long>(), SituacaoProposta.Aprovada))
                .ReturnsAsync(new PropostaMovimentacao { Justificativa = "Proposta excelente, aprovada." });

            _mediator.Setup(m => m.Send(It.IsAny<ObterPerfilAreaPromotoraQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((RetornoListagemDTO)null!);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.PodeEnviar.Should().BeFalse();
            resultado.UltimaJustificativaAprovacaoRecusa.Should().Be("Proposta excelente, aprovada.");
        }

        #region Factory Methods

        private void ConfigurarPerfilEUsuarioLogado(Guid perfilId, string loginUsuario)
        {
            _mediator.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(perfilId);

            _mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new Usuario { Login = loginUsuario });
        }

        private static Proposta CriarPropostaValida()
        {
            return new Proposta
            {
                Id = 1,
                Excluido = false,
                Situacao = SituacaoProposta.Cadastrada,
                Turmas = [],
                Pareceristas = [],
                PublicosAlvo = [],
                FuncoesEspecificas = [],
                PublicoAlvoOutros = string.Empty,
                FuncaoEspecificaOutros = string.Empty
            };
        }

        private void ConfigurarDependenciasComunsParaSucesso(Proposta proposta)
        {
            _repositorioProposta.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(proposta);
            _repositorioProposta.Setup(r => r.ObterDrePorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterPublicoAlvoPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterFuncoesEspecificasPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterCriteriosValidacaoInscricaoPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterVagasRemacenentesPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterPalavrasChavesPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterModalidadesPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterAnosTurmasPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterComponentesCurricularesPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterCriterioCertificacaoPorPropostaId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterTurmasPorId(It.IsAny<long>())).ReturnsAsync(proposta.Turmas ?? []);
            _repositorioProposta.Setup(r => r.ObterTiposInscricaoPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterPareceristasPorId(It.IsAny<long>())).ReturnsAsync(proposta.Pareceristas ?? []);
            _repositorioProposta.Setup(r => r.ObterPropostaPareceristaConsideracaoPorId(It.IsAny<long>())).ReturnsAsync([]);
            _repositorioProposta.Setup(r => r.ObterPropostaTurmasDresPorPropostaTurmaId(It.IsAny<long>())).ReturnsAsync([]);

            _repositorioPropostaMovimentacao.Setup(r => r.ObterUltimoParecerPropostaId(It.IsAny<long>(), It.IsAny<SituacaoProposta>())).ReturnsAsync(new PropostaMovimentacao());
            _repositorioPropostaMovimentacao.Setup(r => r.ObterUltimaJustificativaDevolucao(It.IsAny<long>())).ReturnsAsync(string.Empty);

            _repositorioAreaPromotora.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(new AreaPromotora());

            _mapper.Setup(m => m.Map<PropostaCompletoDTO>(It.IsAny<Proposta>())).Returns(new PropostaCompletoDTO());
            _mapper.Setup(m => m.Map<AuditoriaDTO>(It.IsAny<Proposta>())).Returns(new AuditoriaDTO());
            _mapper.Setup(m => m.Map<PropostaAreaPromotoraDTO>(It.IsAny<AreaPromotora>())).Returns(new PropostaAreaPromotoraDTO());

            _mediator.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());
            _mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Usuario { Login = "12345" });

            _mediator.Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new ParametroSistema { Valor = "5" });
        }

        private static Proposta CriarPropostaExcluida()
        {
            return new Proposta
            {
                Id = 1,
                Excluido = true
            };
        }

        #endregion
    }
}
