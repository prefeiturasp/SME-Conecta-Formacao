using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoFinalizarCodafListaPresencaTestes
    {
        private const long CodafListaPresencaId = 1;
        private const string LoginUsuario = "1234567";
        private const string OutroLogin = "outro-usuario";

        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoFinalizarCodafListaPresenca _casoDeUso;

        public CasoDeUsoFinalizarCodafListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoFinalizarCodafListaPresenca>();
        }

        [Fact]
        public async Task DadoListaPresencaInexistente_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            _contextoAplicacaoMock.SetupGet(c => c.EhAdministrador).Returns(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync((CodafListaPresenca?)null);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().ContainSingle()
                .Which.Should().Be("Lista de presença não encontrada.");
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoPerfilRestritoEListaCriadaPorOutroUsuario_QuandoExecutar_EntaoDeveRetornarErroDenegado()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf, criadoLogin: OutroLogin);
            ConfigurarContexto(ehAdministrador: false, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().ContainSingle()
                .Which.Should().Be("Você não tem permissão para finalizar esta lista de presença.");
            lista.Status.Should().Be(StatusCodafListaPresenca.AguardandoDf);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoListaJaFinalizada_QuandoExecutar_EntaoDeveRetornarErroDeNegocio()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.Finalizado, criadoLogin: LoginUsuario);
            ConfigurarContexto(ehAdministrador: false, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().ContainSingle()
                .Which.Should().Be("Não é possível finalizar uma lista de presença com a situação 'Finalizada'.");
            lista.EstaFinalizado().Should().BeTrue();
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoListaComInscricaoAprovada_QuandoExecutar_EntaoDeveRetornarErroDeNegocio()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf, criadoLogin: LoginUsuario, aprovacoes: true);
            ConfigurarContexto(ehAdministrador: false, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().ContainSingle()
                .Which.Should().Be("Lista de presença só pode ser finalizada se não houver aprovações.");
            lista.EstaFinalizado().Should().BeFalse();
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoListaSemInscricoes_QuandoExecutar_EntaoDeveFinalizarComSucesso()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf, criadoLogin: LoginUsuario);
            ConfigurarContexto(ehAdministrador: false, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.MensagensErro.Should().BeEmpty();
            lista.EstaFinalizado().Should().BeTrue();
            lista.Status.Should().Be(StatusCodafListaPresenca.Finalizado);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(lista), Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public async Task DadoListaComInscricoesNaoAprovadas_QuandoExecutar_EntaoDeveFinalizarComSucesso(bool? aprovado)
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf, criadoLogin: LoginUsuario, aprovacoes: aprovado);
            ConfigurarContexto(ehAdministrador: false, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.MensagensErro.Should().BeEmpty();
            lista.EstaFinalizado().Should().BeTrue();
            lista.Status.Should().Be(StatusCodafListaPresenca.Finalizado);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(lista), Times.Once);
        }

        [Fact]
        public async Task DadoAdministradorEListaCriadaPorOutroUsuario_QuandoExecutar_EntaoDeveFinalizarComSucesso()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf, criadoLogin: OutroLogin);
            ConfigurarContexto(ehAdministrador: true, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            lista.EstaFinalizado().Should().BeTrue();
            lista.Status.Should().Be(StatusCodafListaPresenca.Finalizado);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(lista), Times.Once);
        }

        [Fact]
        public async Task DadoListaComStatusIncompativel_QuandoFinalizar_EntaoDeveRetornarErroDeNegocio()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.Iniciado, criadoLogin: LoginUsuario);
            ConfigurarContexto(ehAdministrador: false, loginUsuario: LoginUsuario);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().ContainSingle()
                .Which.Should().Be("Não foi possível finalizar a lista de presença.");
            lista.EstaFinalizado().Should().BeFalse();
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        private void ConfigurarContexto(bool ehAdministrador, string loginUsuario)
        {
            _contextoAplicacaoMock.SetupGet(c => c.EhAdministrador).Returns(ehAdministrador);
            _contextoAplicacaoMock.SetupGet(c => c.LoginUsuario).Returns(loginUsuario);
        }

        private static CodafListaPresenca CriarLista(
            StatusCodafListaPresenca status,
            string criadoLogin,
            params bool?[] aprovacoes)
        {
            var lista = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                status)
            {
                CriadoLogin = criadoLogin
            };

            foreach (var aprovado in aprovacoes)
            {
                lista.CodafInscricoes.Add(new CodafInscricaoListaPresenca { Aprovado = aprovado });
            }

            return lista;
        }
    }
}
