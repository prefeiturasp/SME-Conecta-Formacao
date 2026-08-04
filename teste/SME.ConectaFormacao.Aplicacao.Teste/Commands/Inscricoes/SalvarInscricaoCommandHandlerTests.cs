using AutoMapper;
using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Email.InscricaoEmEspera;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class SalvarInscricaoCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarInscricaoCommandHandler _handler;
        private readonly Faker _faker;

        public SalvarInscricaoCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<SalvarInscricaoCommandHandler>();
            _faker = new Faker("pt_BR");

            ConfigurarTransacaoMock();
        }

        [Fact]
        public async Task DadoUsuarioNaoLogado_QuandoSalvarInscricao_EntaoDeveLancarExcecao()
        {
            // Arrange
            var comando = GerarComandoValido();

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUsuarioInternoSemCargo_QuandoSalvarInscricao_EntaoDeveLancarExcecao()
        {
            // Arrange
            var comando = GerarComandoValido(cargoCodigo: string.Empty);
            var usuario = GerarUsuarioValido(TipoUsuario.Interno);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));

            // Assert
            Assert.NotNull(excecao);
        }

        [Fact]
        public async Task DadoTurmaNaoEncontrada_QuandoSalvarInscricao_EntaoDeveLancarExcecao()
        {
            // Arrange
            var comando = GerarComandoValido();
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1 };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));

            // Assert
            Assert.NotNull(excecao);
        }

        [Fact]
        public async Task DadoUsuarioJaInscrito_QuandoSalvarInscricao_EntaoDeveLancarExcecao()
        {
            // Arrange
            var comando = GerarComandoValido();
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(propostaTurmaFake.PropostaId, usuario.Id))
                .ReturnsAsync(true);

            // Act
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));

            // Assert
            Assert.NotNull(excecao);
        }

        [Fact]
        public async Task DadoInscricaoVagaRemanescenteValida_QuandoSalvarInscricao_EntaoDevePersistirComSucesso()
        {
            // Arrange
            var comando = GerarComandoValido(vagaRemanescente: true);
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.Sim };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            _mocker.GetMock<IRepositorioInscricao>().Verify(r => r.Inserir(It.IsAny<Inscricao>()), Times.Once);
            _mocker.GetMock<ITransacao>().Verify(t => t.Iniciar(), Times.Once);
        }

        [Fact]
        public async Task DadoInscricaoComTelefoneAtualizado_QuandoSalvarInscricao_EntaoDeveAtualizarCacheDoUsuario()
        {
            // Arrange
            var novoTelefone = _faker.Phone.PhoneNumber("(##) #####-####");
            var comando = GerarComandoValido();
            comando.InscricaoDto.UsuarioTelefone = novoTelefone;

            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            usuario.Telefone = "11999999999";

            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.Sim };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            // Act
            await _handler.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IUsuarioCacheService>().Verify(
                s => s.AtualizarTelefoneEInvalidarCacheAsync(usuario, novoTelefone),
                Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioInternoComCargo_QuandoSalvarInscricao_EntaoDeveMapeCargoFuncao()
        {
            // Arrange
            var comando = GerarComandoValido(cargoCodigo: "123", funcaoCodigo: "456");
            var usuario = GerarUsuarioValido(TipoUsuario.Interno);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id, CargoCodigo = "123", FuncaoCodigo = "456" };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.NaoCursosExtras };

            var cargoFuncoes = new List<CargoFuncao>
            {
                new CargoFuncao { Id = 1, Nome = "Cargo Teste", Tipo = CargoFuncaoTipo.Cargo },
                new CargoFuncao { Id = 2, Nome = "Funcao Teste", Tipo = CargoFuncaoTipo.Funcao }
            };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargoFuncoes);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPublicosAlvosPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaFuncoesEspecificasPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ConfirmarInscricaoVaga(It.IsAny<Inscricao>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IMediator>().Verify(
                m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoFormacaoNaoHomologada_QuandoSalvarInscricao_EntaoDeveConfirmarVaga()
        {
            // Arrange
            var comando = GerarComandoValido();
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.NaoCursosPorIN };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ConfirmarInscricaoVaga(It.IsAny<Inscricao>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioInscricao>().Verify(
                r => r.ConfirmarInscricaoVaga(It.IsAny<Inscricao>()),
                Times.Once);
            _mocker.GetMock<IRepositorioInscricao>().Verify(
                r => r.Atualizar(It.Is<Inscricao>(i => i.Situacao == SituacaoInscricao.Confirmada)),
                Times.Once);
        }

        [Fact]
        public async Task DadoConfirmacaoVagaFalha_QuandoSalvarInscricao_EntaoDeveLancarExcecaoEFazerRollback()
        {
            // Arrange
            var comando = GerarComandoValido();
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.NaoCursosPorIN };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ConfirmarInscricaoVaga(It.IsAny<Inscricao>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));

            _mocker.GetMock<ITransacao>().Verify(t => t.Iniciar(), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioExternoComDreValida_QuandoSalvarInscricao_EntaoDevePersistirComSucesso()
        {
            // Arrange
            var comando = GerarComandoValido();
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            usuario.CodigoEolUnidade = "000001";

            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.Sim };

            var drePropostaTurma = new List<PropostaTurmaDre>
            {
                new PropostaTurmaDre
                {
                    Dre = new Dre { Codigo = "000001", Nome = "DRE Teste", Todos = false }
                }
            };

            var unidadeEol = new UnidadeEol
            {
                Codigo = "000001",
                CodigoReferencia = "000001",
                Tipo = UnidadeEolTipo.Escola
            };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(drePropostaTurma);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUnidadePorCodigoEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            _mocker.GetMock<IRepositorioInscricao>().Verify(r => r.Inserir(It.IsAny<Inscricao>()), Times.Once);
        }

        [Fact]
        public async Task DadoVagaRemanescenteSituacao_QuandoSalvarInscricao_EntaoDeveEnviarEmailEmEspera()
        {
            // Arrange
            var comando = GerarComandoValido(vagaRemanescente: true);
            var usuario = GerarUsuarioValido(TipoUsuario.Externo);
            var inscricaoFake = new Inscricao { PropostaTurmaId = 1, UsuarioId = usuario.Id, Id = 100 };
            var propostaTurmaFake = new PropostaTurma { PropostaId = 10 };
            var propostaFake = new Proposta { Id = 10, FormacaoHomologada = FormacaoHomologada.Sim };

            ConfigurarMockMapeamento(comando.InscricaoDto, inscricaoFake);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurmaFake);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaFake);

            // Act
            await _handler.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IMediator>().Verify(
                m => m.Send(It.IsAny<EnviarEmailInscricaoEmEsperaCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #region Métodos Privados (Helpers)

        private SalvarInscricaoCommand GerarComandoValido(string cargoCodigo = "123", bool vagaRemanescente = false, string funcaoCodigo = "")
        {
            var dto = new InscricaoDto
            {
                PropostaTurmaId = _faker.Random.Long(1, 100),
                CargoCodigo = cargoCodigo,
                VagaRemanescente = vagaRemanescente,
                FuncaoCodigo = funcaoCodigo
            };

            return new SalvarInscricaoCommand(dto);
        }

        private Usuario GerarUsuarioValido(TipoUsuario tipo)
        {
            return new Usuario(_faker.Internet.UserName(), _faker.Person.FullName, _faker.Person.Email)
            {
                Id = _faker.Random.Long(1, 100),
                Tipo = tipo,
                CodigoEolUnidade = _faker.Random.Number(1000, 9999).ToString()
            };
        }

        private void ConfigurarTransacaoMock()
        {
            var dbTransactionMock = new Mock<IDbTransaction>();
            _mocker.GetMock<ITransacao>()
                .Setup(t => t.Iniciar())
                .Returns(dbTransactionMock.Object);
        }

        private void ConfigurarMockMapeamento(InscricaoDto origem, Inscricao destino)
        {
            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<Inscricao>(origem))
                .Returns(destino);
        }

        #endregion
    }
}