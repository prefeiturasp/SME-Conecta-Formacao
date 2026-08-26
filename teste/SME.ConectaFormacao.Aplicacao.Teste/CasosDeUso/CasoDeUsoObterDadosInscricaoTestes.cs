using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    [ExcludeFromCodeCoverage]
    public class CasoDeUsoObterDadosInscricaoTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoObterDadosInscricao _sut;

        public CasoDeUsoObterDadosInscricaoTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoObterDadosInscricao>();
        }

        [Fact]
        public async Task DadoUsuarioExterno_QuandoExecutar_EntaoRetornaDadosInscricaoBasicos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Usuario Externo",
                Login = "12345678901",
                Tipo = TipoUsuario.Externo,
                EmailEducacional = "externo@teste.com",
                Telefone = "11999999999"
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.UsuarioNome.Should().Be("Usuario Externo");
            resultado.UsuarioCpf.Should().Be("123.456.789-01");
            resultado.UsuarioEmail.Should().Be("externo@teste.com");
            resultado.UsuarioRf.Should().Be("12345678901");
            resultado.UsuarioTelefone.Should().Be("11999999999");
            resultado.UsuarioCargos.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoUsuarioInternoSemCargos_QuandoExecutar_EntaoRetornaDadosInscricaoComRfCpf()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Usuario Interno",
                Login = "1234567",
                Tipo = TipoUsuario.Interno,
                EmailEducacional = "interno@teste.com",
                Telefone = "11999999999"
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(q => q.RegistroFuncional == "1234567"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CursistaCargoServicoEol>());

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.UsuarioNome.Should().Be("Usuario Interno");
            // Sem cargos, usa o Login como CPF base
            resultado.UsuarioCpf.Should().Be("000.012.345-67");
            resultado.UsuarioCargos.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoUsuarioInternoComCargosESemCpfNoBanco_QuandoExecutar_EntaoDeveSalvarCpfERetornarCargos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Usuario Interno",
                Login = "1234567",
                Tipo = TipoUsuario.Interno,
                Cpf = "" // Sem CPF no banco
            };

            var cargos = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    Cpf = "12345678901",
                    CdCargoBase = 1,
                    CargoBase = "Cargo 1",
                    CdDreCargoBase = "DRE1",
                    CdUeCargoBase = "UE1",
                    TipoVinculoCargoBase = 1,
                    DataInicioCargoBase = DateTime.Today,

                    CdFuncaoAtividade = 2,
                    FuncaoAtividade = "Funcao 2",
                    CdDreFuncaoAtividade = "DRE2",
                    CdUeFuncaoAtividade = "UE2",
                    TipoVinculoFuncaoAtividade = 2,
                    DataInicioFuncaoAtividade = DateTime.Today,

                    CdCargoSobreposto = 3,
                    CargoSobreposto = "Cargo 3",
                    CdDreCargoSobreposto = "DRE3",
                    CdUeCargoSobreposto = "UE3",
                    TipoVinculoCargoSobreposto = 3,
                    DataInicioCargoSobreposto = DateTime.Today
                }
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(q => q.RegistroFuncional == "1234567"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargos);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.UsuarioCpf.Should().Be("123.456.789-01"); // Masked

            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.Is<SalvarUsuarioCommand>(c => c.Usuario.Cpf == "12345678901"), It.IsAny<CancellationToken>()), Times.Once);

            resultado.UsuarioCargos.Should().HaveCount(2); // Base + Sobreposto

            var cargoBase = resultado.UsuarioCargos.First();
            cargoBase.Codigo.Should().Be("1");
            cargoBase.Descricao.Should().Be("Cargo 1");
            cargoBase.DreCodigo.Should().Be("DRE1");
            cargoBase.UeCodigo.Should().Be("UE1");
            cargoBase.TipoVinculo.Should().Be(1);
            
            cargoBase.Funcoes.Should().HaveCount(1);
            var funcao = cargoBase.Funcoes.First();
            funcao.Codigo.Should().Be("2");
            funcao.Descricao.Should().Be("Funcao 2");
            funcao.DreCodigo.Should().Be("DRE2");
            funcao.UeCodigo.Should().Be("UE2");
            funcao.TipoVinculo.Should().Be(2);

            var cargoSobreposto = resultado.UsuarioCargos.Last();
            cargoSobreposto.Codigo.Should().Be("3");
            cargoSobreposto.Descricao.Should().Be("Cargo 3");
            cargoSobreposto.DreCodigo.Should().Be("DRE3");
            cargoSobreposto.UeCodigo.Should().Be("UE3");
            cargoSobreposto.TipoVinculo.Should().Be(3);
        }
    }
}
