using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandHandler _sut;

        public AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandHandler>();
        }

        [Fact]
        public async Task DadoNenhumCargo_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var comando = new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(1, new List<DadosInscricaoCargoEol>());

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task DadoCargoSemVinculoValido_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var dados = new List<DadosInscricaoCargoEol>
            {
                new() {
                    Codigo = "1",
                    DataInicio = DateTime.Today,
                    TipoVinculo = 0,
                    Descricao = "Descricao",
                    DreCodigo = "DRE1",
                    UeCodigo = "UE1"
                }
            };
            var comando = new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(1, dados);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task DadoInscricaoNaoEncontrada_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var dados = new List<DadosInscricaoCargoEol>
            {
                new() {
                    Codigo = "1",
                    DataInicio = DateTime.Today,
                    TipoVinculo = 1,
                    Descricao = "Descricao",
                    DreCodigo = "DRE1",
                    UeCodigo = "UE1"
                }
            };

            var comando = new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(1, dados);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoInscricaoSemCargoIdComFuncao_QuandoExecutar_EntaoAtualizaCargoEFuncao()
        {
            // Arrange
            var dados = new List<DadosInscricaoCargoEol>
            {
                new() {
                    Codigo = "1",
                    DataInicio = DateTime.Today,
                    TipoVinculo = 1,
                    Descricao = "Descricao",
                    DreCodigo = "DRE1",
                    UeCodigo = "UE1",
                    Funcoes = [
                        new ()
                        {
                            Codigo = "2",
                            DataInicio = DateTime.Today,
                            TipoVinculo = 2,
                            DreCodigo = "DRE2",
                            UeCodigo = "UE2",
                            Descricao = "DescricaoFuncao"
                        }
                    ]
                }
            };
            var comando = new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(1, dados);

            var inscricao = new Inscricao { Id = 1, CargoId = null };

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(m => m.ObterPorId(1))
                .ReturnsAsync(inscricao);

            var cargosEol = new List<CargoFuncao>
            {
                new() { Id = 10, Tipo = CargoFuncaoTipo.Cargo },
                new() { Id = 20, Tipo = CargoFuncaoTipo.Funcao }
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosEol);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            inscricao.CargoId.Should().Be(10);
            inscricao.CargoCodigo.Should().Be("1");
            inscricao.CargoDreCodigo.Should().Be("DRE1");
            inscricao.CargoUeCodigo.Should().Be("UE1");

            inscricao.FuncaoId.Should().Be(20);
            inscricao.FuncaoCodigo.Should().Be("2");
            inscricao.FuncaoDreCodigo.Should().Be("DRE2");
            inscricao.FuncaoUeCodigo.Should().Be("UE2");

            inscricao.TipoVinculo.Should().Be(2);

            _mocker.GetMock<IRepositorioInscricao>().Verify(m => m.Atualizar(inscricao), Times.Once);
        }

        [Fact]
        public async Task DadoInscricaoSemCargoNoEol_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var dados = new List<DadosInscricaoCargoEol>
            {
                new() {
                    Codigo = "1",
                    DataInicio = DateTime.Today,
                    TipoVinculo = 0,
                    Descricao = "Descricao",
                    DreCodigo = "DRE1",
                    UeCodigo = "UE1"
                }
            };
            var comando = new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(1, dados);

            var inscricao = new Inscricao { Id = 1, CargoId = null };

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(m => m.ObterPorId(1))
                .ReturnsAsync(inscricao);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _mocker.GetMock<IRepositorioInscricao>().Verify(m => m.Atualizar(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task DadoInscricaoComCargoIdJaPreenchido_QuandoExecutar_EntaoAtualizaApenasTipoVinculo()
        {
            // Arrange
            var dados = new List<DadosInscricaoCargoEol>
            {
                new() {
                    Codigo = "1",
                    DataInicio = DateTime.Today,
                    TipoVinculo = 1,
                    Descricao = "Descricao",
                    DreCodigo = "DRE1",
                    UeCodigo = "UE1"
                }
            };
            var comando = new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(1, dados);

            var inscricao = new Inscricao { Id = 1, CargoId = 10, TipoVinculo = 0 };

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(m => m.ObterPorId(1))
                .ReturnsAsync(inscricao);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            inscricao.TipoVinculo.Should().Be(1);

            _mocker.GetMock<IMediator>()
                .Verify(m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>())
                , Times.Never);
            _mocker.GetMock<IRepositorioInscricao>().Verify(m => m.Atualizar(inscricao), Times.Once);
        }
    }
}
