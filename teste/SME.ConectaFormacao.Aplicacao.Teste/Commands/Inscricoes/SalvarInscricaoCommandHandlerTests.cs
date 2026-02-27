using AutoMapper;
using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class SalvarInscricaoCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarInscricaoCommandHandler _handler;
        private readonly Faker _faker;

        public SalvarInscricaoCommandHandlerTests()
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

        #region Métodos Privados (Helpers)

        private SalvarInscricaoCommand GerarComandoValido(string cargoCodigo = "123", bool vagaRemanescente = false)
        {
            var dto = new InscricaoDto
            {
                PropostaTurmaId = _faker.Random.Long(1, 100),
                CargoCodigo = cargoCodigo,
                VagaRemanescente = vagaRemanescente
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