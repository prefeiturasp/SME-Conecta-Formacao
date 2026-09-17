using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoFinalizarCodafCursoNaoHomologadoTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> _repositorioCodafCursoNaoHomologadoMock;
        private readonly Mock<ICasoDeUsoEmitirDeclaracaoCodaf> _casoDeUsoEmitirDeclaracaoCodafMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoFinalizarCodafCursoNaoHomologado _casoDeUso;

        public CasoDeUsoFinalizarCodafCursoNaoHomologadoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafCursoNaoHomologadoMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologado>();
            _casoDeUsoEmitirDeclaracaoCodafMock = mocker.GetMock<ICasoDeUsoEmitirDeclaracaoCodaf>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoFinalizarCodafCursoNaoHomologado>();
        }

        [Fact]
        public async Task DadoConfirmacaoCienciaFalsa_QuandoExecutarCasoDeUso_EntaoRetornaErroNegocio()
        {
            // Arrange
            var codafId = 1L;
            var dto = new FinalizarCodafDto { ConfirmacaoCiencia = false };

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId, dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
        }

        [Fact]
        public async Task DadoCodafNaoEncontrado_QuandoExecutarCasoDeUso_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            var codafId = 1L;
            var dto = new FinalizarCodafDto { ConfirmacaoCiencia = true };

            _repositorioCodafCursoNaoHomologadoMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync((CodafCursoNaoHomologado?)null);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId, dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
        }

        [Fact]
        public async Task DadoPerfilRestritoEUsuarioDiferente_QuandoExecutarCasoDeUso_EntaoRetornaErroNegocio()
        {
            // Arrange
            var codafId = 1L;
            var dto = new FinalizarCodafDto { ConfirmacaoCiencia = true };
            var codaf = new CodafCursoNaoHomologado { CriadoLogin = "outro_usuario" };

            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(false);
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("usuario_atual");

            _repositorioCodafCursoNaoHomologadoMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId, dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutarCasoDeUso_EntaoRetornaSucessoEChamaDependencias()
        {
            // Arrange
            var codafId = 1L;
            var dto = new FinalizarCodafDto { ConfirmacaoCiencia = true };
            var codaf = new CodafCursoNaoHomologado { CriadoLogin = "usuario_atual", CodafInscricoes = [new()] };
            codaf.DefinirStatus();

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("usuario_atual");

            _repositorioCodafCursoNaoHomologadoMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(codaf);

            _casoDeUsoEmitirDeclaracaoCodafMock
                .Setup(c => c.ExecutarAsync(codafId))
                .ReturnsAsync(Resultado.DeSucesso());

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId, dto);

            // Assert
            resultado.Sucesso.Should().BeTrue();

            _repositorioCodafCursoNaoHomologadoMock.Verify(r => r.Atualizar(codaf), Times.Once);
            _casoDeUsoEmitirDeclaracaoCodafMock.Verify(c => c.ExecutarAsync(codafId), Times.Once);
        }
    }
}
