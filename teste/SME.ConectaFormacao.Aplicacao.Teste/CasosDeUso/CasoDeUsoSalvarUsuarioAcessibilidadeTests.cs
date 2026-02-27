using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoSalvarUsuarioAcessibilidadeTests
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoSalvarUsuarioAcessibilidade _sut;
        private readonly Faker _faker;

        public CasoDeUsoSalvarUsuarioAcessibilidadeTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoSalvarUsuarioAcessibilidade>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoUsuarioIdNulo_QuandoUsuarioNaoEncontradoPorLogin_EntaoDeveRetornarErroValidacao()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var dto = new UsuarioAcessibilidadeDto { UsuarioId = null };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(login, dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();

            _mocker.GetMock<IUsuarioAcessibilidadeService>()
                .Verify(s => s.SalvarAcessibilidadeDaInscricaoAsync(It.IsAny<UsuarioAcessibilidade>()), Times.Never);
        }

        [Fact]
        public async Task DadoUsuarioIdNulo_QuandoUsuarioEncontradoPorLogin_EntaoDeveSalvarERetornarSucesso()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var dto = new UsuarioAcessibilidadeDto { UsuarioId = null };
            var usuarioIdMocado = _faker.Random.Long(1, 1000);

            var usuario = new Usuario(login, _faker.Person.FullName, _faker.Person.Email)
            {
                Id = usuarioIdMocado
            };

            var entidadeMapeada = new UsuarioAcessibilidade();

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<UsuarioAcessibilidade>(dto))
                .Returns(entidadeMapeada);

            // Act
            var resultado = await _sut.ExecutarAsync(login, dto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            entidadeMapeada.UsuarioId.Should().Be(usuarioIdMocado);

            _mocker.GetMock<IUsuarioAcessibilidadeService>()
                .Verify(s => s.SalvarAcessibilidadeDaInscricaoAsync(entidadeMapeada), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioIdInformado_QuandoExecutar_EntaoNaoDeveBuscarUsuarioPorLoginEDeveSalvarERetornarSucesso()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var dto = new UsuarioAcessibilidadeDto { UsuarioId = _faker.Random.Long(1, 1000) };
            var entidadeMapeada = new UsuarioAcessibilidade();

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<UsuarioAcessibilidade>(dto))
                .Returns(entidadeMapeada);

            // Act
            var resultado = await _sut.ExecutarAsync(login, dto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            entidadeMapeada.UsuarioId.Should().Be(dto.UsuarioId.Value);

            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.ObterPorLogin(It.IsAny<string>()), Times.Never);

            _mocker.GetMock<IUsuarioAcessibilidadeService>()
                .Verify(s => s.SalvarAcessibilidadeDaInscricaoAsync(entidadeMapeada), Times.Once);
        }
    }
}