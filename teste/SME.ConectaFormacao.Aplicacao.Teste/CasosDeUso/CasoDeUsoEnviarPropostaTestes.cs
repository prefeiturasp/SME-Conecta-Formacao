using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEnviarPropostaTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoEnviarProposta _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoEnviarPropostaTestes()
        {
            _mocker = new AutoMocker();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _mapperMock = _mocker.GetMock<IMapper>();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoEnviarProposta>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Proposta)null!);

            // Act
            var acao = () => _casoDeUso.Executar(propostaId);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaComSituacaoInvalida_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var proposta = CriarPropostaBase();
            proposta.Situacao = SituacaoProposta.Rascunho;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            var acao = () => _casoDeUso.Executar(proposta.Id);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.PROPOSTA_NAO_PODE_SER_ENVIADA);

            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaAprovadaHomologadaSemNumeroHomologacao_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var proposta = CriarPropostaBase();
            proposta.Situacao = SituacaoProposta.Aprovada;
            proposta.FormacaoHomologada = FormacaoHomologada.Sim;
            proposta.NumeroHomologacao = 0;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            var acao = () => _casoDeUso.Executar(proposta.Id);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.PROPOSTA_NUMERO_HOMOLOGACAO_DEVE_SER_INFORMADO);
        }

        [Fact]
        public async Task DadoPropostaAutomaticaComFuncaoOutros_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var proposta = CriarPropostaBase();
            proposta.FormacaoHomologada = FormacaoHomologada.NaoCursosExtras;

            ConfigurarConsultasBase(proposta, new[]
            {
                new PropostaTipoInscricao { TipoInscricao = TipoInscricao.Automatica }
            });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExisteCargoFuncaoOutrosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var acao = () => _casoDeUso.Executar(proposta.Id);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.PROPOSTA_JEIF_COM_OUTROS);

            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoDatasInscricaoInvalidas_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var proposta = CriarPropostaBase();
            var mensagemErro = _faker.Lorem.Sentence();

            ConfigurarConsultasBase(proposta, Array.Empty<PropostaTipoInscricao>());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mensagemErro);

            // Act
            var acao = () => _casoDeUso.Executar(proposta.Id);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(mensagemErro);
        }

        [Fact]
        public async Task DadoPropostaNaoHomologadaEUsuarioNaoAdmin_QuandoExecutar_EntaoDevePublicarGeracaoDeVagas()
        {
            // Arrange
            var proposta = CriarPropostaBase();
            proposta.FormacaoHomologada = FormacaoHomologada.NaoCursosExtras;

            ConfigurarConsultasBase(proposta, Array.Empty<PropostaTipoInscricao>());
            ConfigurarComandosDeSucesso();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            var resultado = await _casoDeUso.Executar(proposta.Id);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(
                It.Is<EnviarPropostaCommand>(c => c.PropostaId == proposta.Id && c.Situacao == SituacaoProposta.Publicada),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarPropostaMovimentacaoCommand>(c => c.PropostaId == proposta.Id && c.Situacao == SituacaoProposta.Publicada),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.GerarPropostaTurmaVaga && (long)c.Filtros == proposta.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaHomologadaComPareceristasEAcessoAdmin_QuandoExecutar_EntaoDeveNotificarPareceristas()
        {
            // Arrange
            var proposta = CriarPropostaBase();
            proposta.FormacaoHomologada = FormacaoHomologada.Sim;
            proposta.Situacao = SituacaoProposta.AguardandoAnaliseDf;

            var pareceristas = new[]
            {
                new PropostaParecerista
                {
                    RegistroFuncional = _faker.Random.ReplaceNumbers("######"),
                    NomeParecerista = _faker.Person.FullName,
                    Situacao = SituacaoParecerista.AguardandoValidacao
                }
            };

            var pareceristasDto = new[]
            {
                new PropostaPareceristaResumidoDTO(pareceristas[0].RegistroFuncional!, pareceristas[0].NomeParecerista!)
            };

            ConfigurarConsultasBase(proposta, new[]
            {
                new PropostaTipoInscricao { TipoInscricao = TipoInscricao.Manual }
            }, pareceristas);
            ConfigurarComandosDeSucesso();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExistePareceristasAdicionadosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Perfis.ADMIN_DF);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<PropostaPareceristaResumidoDTO>>(It.IsAny<IEnumerable<PropostaParecerista>>()))
                .Returns(pareceristasDto);

            // Act
            var resultado = await _casoDeUso.Executar(proposta.Id);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(
                It.Is<EnviarPropostaCommand>(c => c.PropostaId == proposta.Id && c.Situacao == SituacaoProposta.AguardandoAnalisePeloParecerista),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c => EhComandoNotificacaoPareceristasValido(c, proposta.Id)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        private static bool EhComandoNotificacaoPareceristasValido(PublicarNaFilaRabbitCommand comando, long propostaId)
        {
            if (comando.Rota != RotasRabbit.NotificarPareceristasSobreAtribuicaoPelaDF)
                return false;

            var notificacao = comando.Filtros as NotificacaoPropostaPareceristasDTO;
            return notificacao is not null
                && notificacao.PropostaId == propostaId
                && notificacao.Pareceristas.Count() == 1;
        }

        private void ConfigurarConsultasBase(Proposta proposta, IEnumerable<PropostaTipoInscricao> tiposInscricao, IEnumerable<PropostaParecerista>? pareceristas = null)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExisteCargoFuncaoOutrosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaTipoInscricaoPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiposInscricao);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPareceristasAdicionadosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas ?? Array.Empty<PropostaParecerista>());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExistePareceristasAdicionadosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
        }

        private void ConfigurarComandosDeSucesso()
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        private Proposta CriarPropostaBase()
        {
            return new Proposta
            {
                Id = _faker.Random.Long(1, 1000),
                NomeFormacao = _faker.Lorem.Sentence(3),
                Situacao = SituacaoProposta.Cadastrada,
                FormacaoHomologada = FormacaoHomologada.NaoCursosExtras,
                DataInscricaoFim = DateTime.Today.AddDays(5),
                DataRealizacaoFim = DateTime.Today.AddDays(10),
                Excluido = false
            };
        }
    }
}
