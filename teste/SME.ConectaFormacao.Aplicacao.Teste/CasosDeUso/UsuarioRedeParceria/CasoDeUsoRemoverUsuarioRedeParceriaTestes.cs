using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuariosRedeParceria;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoRemoverUsuarioRedeParceriaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverUsuarioRedeParceria _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverUsuarioRedeParceriaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverUsuarioRedeParceria>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioInexistente_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            // Act
            var act = () => _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraPorIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoUsuarioNaoEhRedeParceria_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var usuario = new Usuario
            {
                Id = id,
                Login = _faker.Internet.UserName(),
                Tipo = TipoUsuario.Interno
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var act = () => _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraPorIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task DadoFalhaIntegracaoCoreSSO_QuandoExecutar_EntaoDeveLancarNegocioException(bool desvinculadoSucesso, bool inativadoSucesso)
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var areaPromotoraId = _faker.Random.Long(1, 500);
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = id,
                Login = _faker.Internet.UserName(),
                Tipo = TipoUsuario.RedeParceria,
                AreaPromotoraId = areaPromotoraId
            };
            var areaPromotora = new AreaPromotora
            {
                Id = areaPromotoraId,
                GrupoId = grupoId
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == areaPromotoraId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);
            _mediatorMock
                .Setup(m => m.Send(It.Is<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login && c.PerfilId == grupoId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(desvinculadoSucesso);
            _mediatorMock
                .Setup(m => m.Send(It.Is<InativarUsuarioCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inativadoSucesso);

            // Act
            var act = () => _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.ERRO_AO_CRIAR_ATUALIZAR_USUARIO_NO_CORESSO);
            _mediatorMock.Verify(m => m.Send(It.IsAny<UsuarioPossuiPropostaQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoverUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoUsuarioPossuiProposta_QuandoExecutar_EntaoDeveInativarUsuarioSalvarRemoverCacheERetornarMensagemInativado()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var areaPromotoraId = _faker.Random.Long(1, 500);
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = id,
                Login = _faker.Internet.UserName(),
                Tipo = TipoUsuario.RedeParceria,
                AreaPromotoraId = areaPromotoraId,
                Situacao = SituacaoUsuario.Ativo
            };
            var areaPromotora = new AreaPromotora
            {
                Id = areaPromotoraId,
                GrupoId = grupoId
            };
            var nomeChaveCache = CacheDistribuidoNomes.Usuario.Parametros(usuario.Login);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == areaPromotoraId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);
            _mediatorMock
                .Setup(m => m.Send(It.Is<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login && c.PerfilId == grupoId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.Is<InativarUsuarioCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.Is<UsuarioPossuiPropostaQuery>(q => q.Login == usuario.Login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(MensagemNegocio.USUARIO_FOI_INATIVO_POR_POSSUIR_PROPOSTA_CADASTRADA);
            resultado.EntidadeId.Should().Be(id);
            usuario.Situacao.Should().Be(SituacaoUsuario.Inativo);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarUsuarioCommand>(c => c.Usuario == usuario && c.Usuario.Situacao == SituacaoUsuario.Inativo), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoverUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverCacheCommand>(c => c.Chave == nomeChaveCache), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioNaoPossuiProposta_QuandoExecutar_EntaoDeveRemoverUsuarioRemoverCacheERetornarMensagemExcluido()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var areaPromotoraId = _faker.Random.Long(1, 500);
            var grupoId = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = id,
                Login = _faker.Internet.UserName(),
                Tipo = TipoUsuario.RedeParceria,
                AreaPromotoraId = areaPromotoraId,
                Situacao = SituacaoUsuario.Ativo
            };
            var areaPromotora = new AreaPromotora
            {
                Id = areaPromotoraId,
                GrupoId = grupoId
            };
            var nomeChaveCache = CacheDistribuidoNomes.Usuario.Parametros(usuario.Login);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == areaPromotoraId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);
            _mediatorMock
                .Setup(m => m.Send(It.Is<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login && c.PerfilId == grupoId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.Is<InativarUsuarioCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.Is<UsuarioPossuiPropostaQuery>(q => q.Login == usuario.Login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(MensagemNegocio.USUARIO_EXCLUIDO_COM_SUCESSO);
            resultado.EntidadeId.Should().Be(id);
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverUsuarioCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverCacheCommand>(c => c.Chave == nomeChaveCache), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
