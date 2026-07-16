using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterDadosInscricaoTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterDadosInscricao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterDadosInscricaoTestes()
        {
            _mocker = new AutoMocker();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoObterDadosInscricao>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoUsuarioInternoComCargosNoEol_QuandoExecutar_EntaoDeveRetornarCargosEAtualizarCpf()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = _faker.Person.FullName,
                Login = "12345678901",
                EmailEducacional = _faker.Internet.Email(),
                Telefone = _faker.Phone.PhoneNumber(),
                Tipo = TipoUsuario.Interno,
                Cpf = string.Empty
            };

            var cargos = new[]
            {
                new CursistaCargoServicoEol
                {
                    Cpf = "12345678901",
                    CdCargoBase = 100,
                    CargoBase = "Professor",
                    CdDreCargoBase = "DRE1",
                    CdUeCargoBase = "UE1",
                    TipoVinculoCargoBase = 1,
                    DataInicioCargoBase = DateTime.Today.AddYears(-1),
                    CdCargoSobreposto = 200,
                    CargoSobreposto = "Coordenador",
                    CdDreCargoSobreposto = "DRE2",
                    CdUeCargoSobreposto = "UE2",
                    TipoVinculoCargoSobreposto = 2,
                    DataInicioCargoSobreposto = DateTime.Today.AddMonths(-6),
                    CdFuncaoAtividade = 300,
                    FuncaoAtividade = "POEI",
                    CdDreFuncaoAtividade = "DRE3",
                    CdUeFuncaoAtividade = "UE3",
                    TipoVinculoFuncaoAtividade = 3,
                    DataInicioFuncaoAtividade = DateTime.Today.AddMonths(-3)
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargos);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.UsuarioNome.Should().Be(usuario.Nome);
            resultado.UsuarioRf.Should().Be(usuario.Login);
            resultado.UsuarioCpf.Should().Be("123.456.789-01");
            resultado.UsuarioCargos.Should().HaveCount(2);
            resultado.UsuarioCargos.First().Funcoes.Should().ContainSingle();

            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarUsuarioCommand>(c => c.Usuario.Cpf == "12345678901"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioExterno_QuandoExecutar_EntaoNaoDeveConsultarCargosNoEol()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = _faker.Person.FullName,
                Login = "12345678901",
                EmailEducacional = _faker.Internet.Email(),
                Telefone = _faker.Phone.PhoneNumber(),
                Tipo = TipoUsuario.Externo
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.UsuarioNome.Should().Be(usuario.Nome);
            resultado.UsuarioCpf.Should().Be("123.456.789-01");
            resultado.UsuarioCargos.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
