using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class AlterarCargoFuncaoVinculoInscricaoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricao;
        private readonly Mock<IMediator> _mediator;
        private readonly AlterarCargoFuncaoVinculoInscricaoCommandHandler _sut;

        public AlterarCargoFuncaoVinculoInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioInscricao = mocker.GetMock<IRepositorioInscricao>();
            _mediator = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<AlterarCargoFuncaoVinculoInscricaoCommandHandler>();
        }

        [Fact]
        public void DadoRepositorioInscricaoNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IRepositorioInscricao repositorioNulo = null!;

            // Act
            var act = () => new AlterarCargoFuncaoVinculoInscricaoCommandHandler(repositorioNulo, _mediator.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("repositorioInscricao");
        }

        [Fact]
        public void DadoMediatorNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IMediator mediatorNulo = null!;

            // Act
            var act = () => new AlterarCargoFuncaoVinculoInscricaoCommandHandler(_repositorioInscricao.Object, mediatorNulo);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("mediator");
        }

        [Fact]
        public async Task DadoInscricaoInexistente_QuandoProcessarComando_EntaoDeveLancarNegocioExceptionNotFound()
        {
            // Arrange
            var comando = CriarComandoValido();
            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync((Inscricao)null!);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.StatusCode.Should().Be((int)System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DadoNenhumCargoRetornadoDoEol_QuandoProcessarComando_EntaoDeveRetornarFalse()
        {
            // Arrange
            var comando = CriarComandoValido();
            var inscricao = new Inscricao { Id = 1 };

            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(inscricao);

            ConfigurarUsuarioLogado("12345");
            ConfigurarCargosEol([]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _repositorioInscricao.Verify(r => r.Atualizar(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task DadoCargoDoComandoNaoEncontradoNoEol_QuandoProcessarComando_EntaoDeveRetornarFalse()
        {
            // Arrange
            var comando = CriarComandoValido("999", 1); // Solicitando cargo 999
            var inscricao = new Inscricao { Id = 1 };

            var cargosEol = new List<CursistaCargoServicoEol>
            {
                new() { CdCargoBase = 111, CargoBase = "Prof", TipoVinculoCargoBase = 1 } // Retorna cargo 111
            };

            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(inscricao);

            ConfigurarUsuarioLogado("12345");
            ConfigurarCargosEol(cargosEol);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _repositorioInscricao.Verify(r => r.Atualizar(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task DadoCargoCorrespondenteNaoLocalizadoNoBancoDeDados_QuandoProcessarComando_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValido("123", 1);
            var inscricao = new Inscricao { Id = 1 };

            var cargosEol = new List<CursistaCargoServicoEol>
            {
                new() { CdCargoBase = 123, CargoBase = "Prof", TipoVinculoCargoBase = 1 }
            };

            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(inscricao);

            ConfigurarUsuarioLogado("12345");
            ConfigurarCargosEol(cargosEol);

            // Simula que o banco não tem mapeamento para este cargo
            ConfigurarCargosFuncoesBanco([]);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NegocioException>();
            _repositorioInscricao.Verify(r => r.Atualizar(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task DadoCargoNaoPermitidoNoPublicoAlvoDaProposta_QuandoProcessarComando_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = CriarComandoValido("123", 1);
            var inscricao = new Inscricao { Id = 1, PropostaTurmaId = 10 };

            var cargosEol = new List<CursistaCargoServicoEol>
            {
                new() { CdCargoBase = 123, CargoBase = "Prof", TipoVinculoCargoBase = 1 }
            };

            var cargoBanco = new CargoFuncao { Id = 50, Tipo = CargoFuncaoTipo.Cargo };

            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(inscricao);

            ConfigurarUsuarioLogado("12345");
            ConfigurarCargosEol(cargosEol);
            ConfigurarCargosFuncoesBanco([cargoBanco]);

            // Configura a proposta exigindo o CargoId 99 (diferente de 50)
            ConfigurarValidacoesProposta(10, [new() { CargoFuncaoId = 99 }], []);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
        }

        [Fact]
        public async Task DadoCargoValidoSemFuncao_QuandoProcessarComando_EntaoDeveAtualizarInscricaoERetornarTrue()
        {
            // Arrange
            var comando = CriarComandoValido("123", 1);
            var inscricao = new Inscricao { Id = 1, PropostaTurmaId = 10 };

            var cargosEol = new List<CursistaCargoServicoEol>
            {
                new() { CdCargoBase = 123, CargoBase = "Prof", TipoVinculoCargoBase = 1, CdDreCargoBase = "DRE-1", CdUeCargoBase = "UE-1" }
            };

            var cargoBanco = new CargoFuncao { Id = 50, Tipo = CargoFuncaoTipo.Cargo };

            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(inscricao);

            ConfigurarUsuarioLogado("12345");
            ConfigurarCargosEol(cargosEol);
            ConfigurarCargosFuncoesBanco([cargoBanco]);

            // Configura proposta permitindo o CargoId 50
            ConfigurarValidacoesProposta(10, [new() { CargoFuncaoId = 50 }], []);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            inscricao.CargoId.Should().Be(50);
            inscricao.CargoCodigo.Should().Be("123");
            inscricao.CargoDreCodigo.Should().Be("DRE-1");
            inscricao.CargoUeCodigo.Should().Be("UE-1");
            inscricao.TipoVinculo.Should().Be(1);

            _repositorioInscricao.Verify(r => r.Atualizar(inscricao), Times.Once);
        }

        [Fact]
        public async Task DadoCargoEFuncaoValidosEInscricaoComFuncaoPrevia_QuandoProcessarComando_EntaoDeveAtualizarAmbosERetornarTrue()
        {
            // Arrange
            var comando = CriarComandoValido("123", 1);
            var inscricao = new Inscricao { Id = 1, PropostaTurmaId = 10, FuncaoId = 999 }; // Possui função prévia

            var cargosEol = new List<CursistaCargoServicoEol>
            {
                new()
                {
                    CdCargoBase = 123, CargoBase = "Prof", TipoVinculoCargoBase = 1, CdDreCargoBase = "DRE-1", CdUeCargoBase = "UE-1",
                    CdFuncaoAtividade = 456, FuncaoAtividade = "Diretor", TipoVinculoFuncaoAtividade = 1, CdDreFuncaoAtividade = "DRE-2", CdUeFuncaoAtividade = "UE-2"
                }
            };

            var cargoBanco = new CargoFuncao { Id = 50, Tipo = CargoFuncaoTipo.Cargo };
            var funcaoBanco = new CargoFuncao { Id = 60, Tipo = CargoFuncaoTipo.Funcao };

            _repositorioInscricao.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(inscricao);

            ConfigurarUsuarioLogado("12345");
            ConfigurarCargosEol(cargosEol);
            ConfigurarCargosFuncoesBanco([cargoBanco, funcaoBanco]);

            // Permite Cargo 50 e Função 60
            ConfigurarValidacoesProposta(10, [new() { CargoFuncaoId = 50 }], [new() { CargoFuncaoId = 60 }]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            inscricao.CargoId.Should().Be(50);
            inscricao.CargoCodigo.Should().Be("123");

            inscricao.FuncaoId.Should().Be(60);
            inscricao.FuncaoCodigo.Should().Be("456");
            inscricao.FuncaoDreCodigo.Should().Be("DRE-2");
            inscricao.FuncaoUeCodigo.Should().Be("UE-2");

            _repositorioInscricao.Verify(r => r.Atualizar(inscricao), Times.Once);
        }

        #region Factory Methods

        private static AlterarCargoFuncaoVinculoInscricaoCommand CriarComandoValido(string cargoCodigo = "123", int tipoVinculo = 1)
        {
            return new AlterarCargoFuncaoVinculoInscricaoCommand(
                1,
                new AlterarCargoFuncaoVinculoIncricaoDTO
                {
                    CargoCodigo = cargoCodigo,
                    TipoVinculo = tipoVinculo
                });
        }

        private void ConfigurarUsuarioLogado(string login)
        {
            _mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new Usuario { Login = login });
        }

        private void ConfigurarCargosEol(List<CursistaCargoServicoEol> cargos)
        {
            _mediator.Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(cargos);
        }

        private void ConfigurarCargosFuncoesBanco(List<CargoFuncao> cargosFuncoes)
        {
            _mediator.Setup(m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(cargosFuncoes);
        }

        private void ConfigurarValidacoesProposta(
            long turmaId,
            List<PropostaPublicoAlvo> publicosAlvo,
            List<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            var turma = new PropostaTurma { Id = turmaId, PropostaId = 999 };

            _mediator.Setup(m => m.Send(It.Is<ObterPropostaTurmaPorIdQuery>(q => q.PropostaTurmaId == turmaId), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(turma);

            _mediator.Setup(m => m.Send(It.Is<ObterPropostaPublicosAlvosPorIdQuery>(q => q.PropostaId == turma.PropostaId), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(publicosAlvo);

            _mediator.Setup(m => m.Send(It.Is<ObterPropostaFuncoesEspecificasPorIdQuery>(q => q.PropostaId == turma.PropostaId), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(funcoesEspecificas);
        }

        #endregion
    }
}
