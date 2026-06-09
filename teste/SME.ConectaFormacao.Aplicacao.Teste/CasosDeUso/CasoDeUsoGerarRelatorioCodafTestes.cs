using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarRelatorioCodafTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _mockRepositorioCodaf;
        private readonly Mock<IServicoRelatorio> _mockServicoRelatorio;
        private readonly CasoDeUsoGerarRelatorioCodaf _sut;

        public CasoDeUsoGerarRelatorioCodafTestes()
        {
            var mocker = new AutoMocker();

            _mockRepositorioCodaf = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _mockServicoRelatorio = mocker.GetMock<IServicoRelatorio>();
            _sut = mocker.CreateInstance<CasoDeUsoGerarRelatorioCodaf>();
        }

        [Fact]
        public async Task DadoCodafInexistente_QuandoExecutar_DeveRetornarNaoEncontrado()
        {
            // Arrange
            long codafId = 1;

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            _mockServicoRelatorio.Verify(s => s.GerarRelatorioCodafAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoCodafValido_QuandoExecutar_DeveRetornarArquivo()
        {
            // Arrange
            long codafId = 1;
            var listaPresenca = new CodafListaPresenca(codafId, 1, new(null, null, null, null, null, null, null), null)
            {
                Proposta = new Proposta { NumeroHomologacao = 12345 },
                PropostaTurma = new PropostaTurma { Nome = "Turma A" }
            };
            _mockRepositorioCodaf.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);
            _mockServicoRelatorio.Setup(s => s.GerarRelatorioCodafAsync(codafId))
                .ReturnsAsync(Encoding.UTF8.GetBytes("conteudo do relatorio"));

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Nome.Should().Be($"CODAF_12345-Turma A.xlsx");
            resultado.Dados.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Fact]
        public async Task DadoCodafComStatusAguardandoDf_QuandoGerarRelatorioCodaf_DeveAtualizarStatusParaFinalizado()
        {
            // Arrange
            long codafId = 1;
            var listaPresenca = new CodafListaPresenca(codafId, 1, new(null, null, null, null, null, null, null), null)
            {
                Proposta = new Proposta { NumeroHomologacao = 12345 },
                PropostaTurma = new PropostaTurma { Nome = "Turma A" }
            };
            listaPresenca.Iniciar();
            listaPresenca.MarcarComoEnviadaParaDf();
            _mockRepositorioCodaf.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);
            _mockServicoRelatorio.Setup(s => s.GerarRelatorioCodafAsync(codafId))
                .ReturnsAsync(Encoding.UTF8.GetBytes("conteudo do relatorio"));
            // Act
            var resultado = await _sut.ExecutarAsync(codafId);
            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mockRepositorioCodaf.Verify(r => r.Atualizar(It.Is<CodafListaPresenca>(c => c.Status == StatusCodafListaPresenca.Finalizado)), Times.Once);
        }

        [Fact]
        public async Task DadoCodafComStatusFinalizado_QuandoGerarRelatorioCodaf_NaoDeveAtualizarStatus()
        {
            // Arrange
            long codafId = 1;
            var listaPresenca = new CodafListaPresenca(codafId, 1, new(null, null, null, null, null, null, null), null)
            {
                Proposta = new Proposta { NumeroHomologacao = 12345 },
                PropostaTurma = new PropostaTurma { Nome = "Turma A" }
            };
            listaPresenca.Iniciar();
            listaPresenca.MarcarComoEnviadaParaDf();
            listaPresenca.Finalizar();
            _mockRepositorioCodaf.Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(codafId))
                .ReturnsAsync(listaPresenca);
            _mockServicoRelatorio.Setup(s => s.GerarRelatorioCodafAsync(codafId))
                .ReturnsAsync(Encoding.UTF8.GetBytes("conteudo do relatorio"));
            // Act
            var resultado = await _sut.ExecutarAsync(codafId);
            // Assert
            resultado.Sucesso.Should().BeTrue();
            _mockRepositorioCodaf.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }
    }
}