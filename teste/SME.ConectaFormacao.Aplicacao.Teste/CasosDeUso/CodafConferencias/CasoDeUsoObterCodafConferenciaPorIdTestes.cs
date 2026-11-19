using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoObterCodafConferenciaPorIdTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterCodafConferenciaPorId _sut;

        public CasoDeUsoObterCodafConferenciaPorIdTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _mapperMock = mocker.GetMock<IMapper>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _sut = mocker.CreateInstance<CasoDeUsoObterCodafConferenciaPorId>();
        }

        [Fact]
        public async Task DadoCodafNaoEncontrado_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            _repositorioMock.Setup(r => r.ObterPorIdDetalhadoAsync(1)).ReturnsAsync((CodafConferencia?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(1);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDiferente_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var codaf = new CodafConferencia(1, 2);
            var dto = new CodafConferenciaDetalhadoDto { Id = 1, CriadoLogin = "outro" };
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid());
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("atual");
            _repositorioMock.Setup(r => r.ObterPorIdDetalhadoAsync(1)).ReturnsAsync(codaf);
            _mapperMock.Setup(m => m.Map<CodafConferenciaDetalhadoDto>(codaf)).Returns(dto);

            // Act
            var resultado = await _sut.ExecutarAsync(1);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Você não tem permissão para visualizar este codaf.");
        }

        [Fact]
        public async Task DadoPerfilAdminComAnexos_QuandoExecutar_EntaoDevePreencherUrlDownloadERetornarSucesso()
        {
            // Arrange
            var arquivoCodigo = Guid.NewGuid();
            var codaf = new CodafConferencia(1, 2);
            var dto = new CodafConferenciaDetalhadoDto
            {
                Id = 1,
                CriadoLogin = "outro",
                Anexos =
                [
                    new() { Id = 10, ArquivoCodigo = arquivoCodigo, NomeArquivo = "doc.pdf", Extensao = ".pdf" }
                ]
            };

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
            _repositorioMock.Setup(r => r.ObterPorIdDetalhadoAsync(1)).ReturnsAsync(codaf);
            _mapperMock.Setup(m => m.Map<CodafConferenciaDetalhadoDto>(codaf)).Returns(dto);
            _servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(arquivoCodigo.ToString(), false))
                .ReturnsAsync("https://storage/doc.pdf");

            // Act
            var resultado = await _sut.ExecutarAsync(1);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados!.Anexos.Should().ContainSingle().Which.UrlDownload.Should().Be("https://storage/doc.pdf");
        }

        [Fact]
        public async Task DadoPerfilEmforpefSemAnexos_QuandoExecutar_EntaoDeveRetornarSucesso()
        {
            // Arrange
            var codaf = new CodafConferencia(1, 2);
            var dto = new CodafConferenciaDetalhadoDto { Id = 1, CriadoLogin = "outro", Anexos = null };
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.EMFORPEF);
            _repositorioMock.Setup(r => r.ObterPorIdDetalhadoAsync(1)).ReturnsAsync(codaf);
            _mapperMock.Setup(m => m.Map<CodafConferenciaDetalhadoDto>(codaf)).Returns(dto);

            // Act
            var resultado = await _sut.ExecutarAsync(1);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados!.Id.Should().Be(1);
        }
    }
}
