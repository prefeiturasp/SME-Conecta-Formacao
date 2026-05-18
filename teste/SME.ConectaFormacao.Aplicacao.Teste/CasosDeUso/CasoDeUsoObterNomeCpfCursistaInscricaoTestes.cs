using AutoMapper;
using Bogus;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes.ObterCargosFuncoesPorUsuarioId;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterNomeCpfCursistaInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoObterNomeCpfCpfCursistaInscricao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterNomeCpfCursistaInscricaoTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker("pt_BR");
            _casoDeUso = new CasoDeUsoObterNomeCpfCpfCursistaInscricao(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Dado_CpfValido_UsuarioEncontrado_Quando_Executar_Entao_RetornarDtoComCargosDoDb()
        {
            // Arrange
            var cpf = _faker.Random.ReplaceNumbers("###########");
            var usuario = new Usuario { Id = _faker.Random.Long(1, 999), Nome = _faker.Name.FullName(), Cpf = cpf, Login = cpf };
            var cargos = new List<DadosInscricaoCargoEol> { new() { Descricao = "Professor", DreCodigo = "108100", UeCodigo = "307556" } };
            var dtoEsperado = new RetornoUsuarioCpfNomeDTO { Nome = usuario.Nome, Cpf = usuario.Cpf };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mapperMock
                .Setup(m => m.Map<RetornoUsuarioCpfNomeDTO>(usuario))
                .Returns(dtoEsperado);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCargosFuncoesPorUsuarioIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargos);

            // Act
            var resultado = await _casoDeUso.Executar(null, cpf);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuario.Nome, resultado.Nome);
            Assert.Equal(cargos, resultado.UsuarioCargos);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterCargosFuncoesPorUsuarioIdQuery>(q => q.UsuarioId == usuario.Id),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Dado_CpfValido_UsuarioNaoEncontrado_Quando_Executar_Entao_LancarNegocioException()
        {
            // Arrange
            var cpf = _faker.Random.ReplaceNumbers("###########");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(null, cpf));

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterCargosFuncoesPorUsuarioIdQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Dado_CpfValido_UsuarioSemNome_Quando_Executar_Entao_LancarNegocioException()
        {
            // Arrange
            var cpf = _faker.Random.ReplaceNumbers("###########");
            var usuario = new Usuario { Id = 1, Nome = null, Cpf = cpf };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(null, cpf));
        }

        [Fact]
        public async Task Dado_RegistroFuncional_EolRetornaCursista_Quando_Executar_Entao_RetornarDadosEolComCargosEol()
        {
            // Arrange
            var rf = _faker.Random.ReplaceNumbers("#######");
            var usuarioLocal = new Usuario { Id = 1, Nome = _faker.Name.FullName(), Cpf = "11111111111", Login = rf };
            var cursistaEol = new RetornoUsuarioCpfNomeDTO { Nome = _faker.Name.FullName(), Cpf = "22222222222" };
            var dtoMapeadoEol = new RetornoUsuarioCpfNomeDTO { Nome = cursistaEol.Nome, Cpf = cursistaEol.Cpf };
            var cargosEol = new List<CursistaCargoServicoEol>
            {
                new() { CdCargoBase = 3085, CargoBase = "Diretor", CdDreCargoBase = "108100", CdUeCargoBase = "307556", TipoVinculoCargoBase = 1 }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLocal);

            _mapperMock
                .Setup(m => m.Map<RetornoUsuarioCpfNomeDTO>(usuarioLocal))
                .Returns(new RetornoUsuarioCpfNomeDTO { Nome = usuarioLocal.Nome, Cpf = usuarioLocal.Cpf });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursistaEol);

            _mapperMock
                .Setup(m => m.Map<RetornoUsuarioCpfNomeDTO>(cursistaEol))
                .Returns(dtoMapeadoEol);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosEol);

            // Act
            var resultado = await _casoDeUso.Executar(rf, null);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(cursistaEol.Nome, resultado.Nome);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterCargosFuncoesPorUsuarioIdQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Dado_RegistroFuncional_EolNaoRetornaCursista_UsuarioLocalExiste_Quando_Executar_Entao_RetornarDadosLocal()
        {
            // Arrange
            var rf = _faker.Random.ReplaceNumbers("#######");
            var usuarioLocal = new Usuario { Id = 1, Nome = _faker.Name.FullName(), Cpf = "11111111111", Login = rf };
            var dtoLocal = new RetornoUsuarioCpfNomeDTO { Nome = usuarioLocal.Nome, Cpf = usuarioLocal.Cpf };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLocal);

            _mapperMock
                .Setup(m => m.Map<RetornoUsuarioCpfNomeDTO>(usuarioLocal))
                .Returns(dtoLocal);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RetornoUsuarioCpfNomeDTO { Nome = null, Cpf = null });

            // Act
            var resultado = await _casoDeUso.Executar(rf, null);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuarioLocal.Nome, resultado.Nome);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Dado_RegistroFuncional_UsuarioLocalNaoExiste_EolNaoRetorna_Quando_Executar_Entao_LancarNegocioException()
        {
            // Arrange
            var rf = _faker.Random.ReplaceNumbers("#######");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RetornoUsuarioCpfNomeDTO { Nome = null, Cpf = null });

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(rf, null));
        }
    }
}
