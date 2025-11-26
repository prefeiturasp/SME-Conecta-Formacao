using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorEolUnidade;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorUnidadeEol;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterUsuariosPorEolUnidadeQueryHandlerTestes
    {
        private readonly Mock<IRepositorioUsuario> _repositorioUsuario;
        private readonly Mock<IMapper> _mapper;
        private readonly ObterUsuariosPorEolUnidadeQueryHandler _handler;
        private readonly Faker _faker;

        public ObterUsuariosPorEolUnidadeQueryHandlerTestes()
        {
            var mocker = new AutoMocker();
            _faker = new();

            _repositorioUsuario = mocker.GetMock<IRepositorioUsuario>();
            _mapper = mocker.GetMock<IMapper>();

            _handler = mocker.CreateInstance<ObterUsuariosPorEolUnidadeQueryHandler>();
        }

        [Fact]
        public async Task DadoUmaRequisicaoComCodigoEolUnidade_QuandoChamarHandler_DeveRetornarDadosDoUsuarioCorretamente()
        {
            // Arrange
            var codigoEolUnidade = "019718";
            var queryRequest = new ObterUsuariosPorEolUnidadeQuery(codigoEolUnidade);
            var usuarios = new List<Usuario>
            {
                new ()
                {
                    Login = _faker.Person.Cpf(false),
                    Nome = _faker.Person.FirstName
                }
            };

            var dadosLoginUsuarioDto = new List<DadosLoginUsuarioDto>
            {
                new ()
                {
                    Login = usuarios.First().Login,
                    Nome = usuarios.First().Nome
                }
            };

            _repositorioUsuario
                .Setup(r => r.ObterUsuariosPorEolUnidadeAsync(codigoEolUnidade, null, null))
                .ReturnsAsync(usuarios);

            _mapper.Setup(m => m.Map<IEnumerable<DadosLoginUsuarioDto>>(It.IsAny<IEnumerable<Usuario>>())).Returns(dadosLoginUsuarioDto);

            // Act
            var resposta = await _handler.Handle(queryRequest, It.IsAny<CancellationToken>());

            // Assert
            resposta.Should().NotBeNull();
            resposta.Should().BeEquivalentTo(dadosLoginUsuarioDto);
        }

        [Fact]
        public async Task DadoUmaRequisicaoComLogin_QuandoChamarHandler_DeveRetornarDadosDoUsuarioCorretamente()
        {
            // Arrange
            var codigoEolUnidade = "019718";
            var login = _faker.Person.Cpf(true);
            var loginSoNumeros = login.SomenteNumeros();
            var queryRequest = new ObterUsuariosPorEolUnidadeQuery(codigoEolUnidade, login);
            var usuarios = new List<Usuario>
            {
                new ()
                {
                    Login = loginSoNumeros,
                    Nome = _faker.Person.FirstName
                }
            };

            var dadosLoginUsuarioDto = new List<DadosLoginUsuarioDto>
            {
                new ()
                {
                    Login = usuarios.First().Login,
                    Nome = usuarios.First().Nome
                }
            };

            _repositorioUsuario
                .Setup(r => r.ObterUsuariosPorEolUnidadeAsync(codigoEolUnidade, loginSoNumeros, null))
                .ReturnsAsync(usuarios);

            _mapper.Setup(m => m.Map<IEnumerable<DadosLoginUsuarioDto>>(It.IsAny<IEnumerable<Usuario>>())).Returns(dadosLoginUsuarioDto);

            // Act
            var resposta = await _handler.Handle(queryRequest, It.IsAny<CancellationToken>());

            // Assert
            resposta.Should().NotBeNull();
            resposta.Should().BeEquivalentTo(dadosLoginUsuarioDto);
        }

        [Fact]
        public async Task DadoUmaRequisicaoComNome_QuandoChamarHandler_DeveRetornarDadosDoUsuarioCorretamente()
        {
            // Arrange
            var codigoEolUnidade = "019718";
            var nome = " Diego  ";
            var queryRequest = new ObterUsuariosPorEolUnidadeQuery(codigoEolUnidade, nome: nome);
            var usuarios = new List<Usuario>
            {
                new ()
                {
                    Login = _faker.Person.Cpf(false),
                    Nome = "Diego Ferreira Moreno"
                }
            };

            var dadosLoginUsuarioDto = new List<DadosLoginUsuarioDto>
            {
                new ()
                {
                    Login = usuarios.First().Login,
                    Nome = usuarios.First().Nome
                }
            };

            _repositorioUsuario
                .Setup(r => r.ObterUsuariosPorEolUnidadeAsync(codigoEolUnidade, null, "diego"))
                .ReturnsAsync(usuarios);

            _mapper.Setup(m => m.Map<IEnumerable<DadosLoginUsuarioDto>>(It.IsAny<IEnumerable<Usuario>>())).Returns(dadosLoginUsuarioDto);

            // Act
            var resposta = await _handler.Handle(queryRequest, It.IsAny<CancellationToken>());

            // Assert
            resposta.Should().NotBeNull();
            resposta.Should().BeEquivalentTo(dadosLoginUsuarioDto);
        }
    }
}
