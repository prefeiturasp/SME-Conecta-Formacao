using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Reflection;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarRelatorioCodafTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IGeradorRelatorioCodafExcelService> _geradorRelatorioMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoGerarRelatorioCodaf _sut;
        private readonly Faker _faker;

        public CasoDeUsoGerarRelatorioCodafTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _geradorRelatorioMock = mocker.GetMock<IGeradorRelatorioCodafExcelService>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            
            _sut = mocker.CreateInstance<CasoDeUsoGerarRelatorioCodaf>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoExistente_QuandoChamarExecutar_EntaoDeveRetornarNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync((CodafListaPresenca?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDiferente_QuandoChamarExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var loginDiferente = "usuario123";
            var meuLogin = "meulogin456";
            
            var listaPresenca = new CodafListaPresenca();
            listaPresenca.GetType().GetProperty("CriadoLogin", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)?.SetValue(listaPresenca, loginDiferente);
            
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid()); // Perfil restrito
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns(meuLogin);
            
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Você não tem permissão para gerar relatório desta lista de presença.");
        }

        [Fact]
        public async Task DadoDadosRelatorioNaoExistente_QuandoChamarExecutar_EntaoDeveRetornarNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var listaPresenca = new CodafListaPresenca();
            listaPresenca.GetType().GetProperty("CriadoLogin", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)?.SetValue(listaPresenca, "meulogin");
            
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid()); // Perfil restrito
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("meulogin");
            
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);
                
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterDadosRelatorioAsync(codafId))
                .ReturnsAsync((DadosPrincipaisRelatorioCodafDto?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Nenhuma informação encontrada para o codaf informado.");
        }

        [Fact]
        public async Task DadoDadosValidos_EStatusNaoFinalizado_QuandoChamarExecutar_EntaoDeveGerarRelatorioEAtualizarStatus()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var listaPresenca = new CodafListaPresenca 
            { 
                Proposta = new Proposta { NumeroHomologacao = 999 },
                PropostaTurma = new PropostaTurma { Nome = "Turma X" }
            };
            typeof(CodafListaPresenca).GetProperty("Status")?.SetValue(listaPresenca, StatusCodafListaPresenca.AguardandoDf);
            
            var dadosRelatorio = new DadosPrincipaisRelatorioCodafDto();
            var bytesRelatorio = new byte[] { 1, 2, 3 };

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
            
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);

            _repositorioCodafListaPresencaMock.Setup(r => r.ObterDadosRelatorioAsync(codafId))
                .ReturnsAsync(dadosRelatorio);

            _geradorRelatorioMock.Setup(g => g.GerarRelatorio(dadosRelatorio, false))
                .Returns(bytesRelatorio);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            resultado.Dados.Nome.Should().Be("CODAF_999-Turma X.xlsx");
            
            listaPresenca.Status.Should().Be(StatusCodafListaPresenca.Finalizado);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(listaPresenca), Times.Once);
        }

        [Fact]
        public async Task DadoDadosValidos_EStatusJaFinalizado_QuandoChamarExecutar_EntaoDeveGerarRelatorioMasNaoAtualizarStatus()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var listaPresenca = new CodafListaPresenca 
            { 
                Proposta = new Proposta { NumeroHomologacao = 999 },
                PropostaTurma = new PropostaTurma { Nome = "Turma X" }
            };
            typeof(CodafListaPresenca).GetProperty("Status")?.SetValue(listaPresenca, StatusCodafListaPresenca.Finalizado);
            
            var dadosRelatorio = new DadosPrincipaisRelatorioCodafDto();
            var bytesRelatorio = new byte[] { 1, 2, 3 };

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.EMFORPEF);
            
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);

            _repositorioCodafListaPresencaMock.Setup(r => r.ObterDadosRelatorioAsync(codafId))
                .ReturnsAsync(dadosRelatorio);

            _geradorRelatorioMock.Setup(g => g.GerarRelatorio(dadosRelatorio, false))
                .Returns(bytesRelatorio);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(listaPresenca), Times.Never);
        }
    }
}