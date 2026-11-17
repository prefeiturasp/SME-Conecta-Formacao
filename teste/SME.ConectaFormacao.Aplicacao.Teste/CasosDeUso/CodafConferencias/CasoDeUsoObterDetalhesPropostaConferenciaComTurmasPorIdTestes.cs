using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorIdTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioMock;
        private readonly CasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId _sut;

        public CasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorIdTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _sut = mocker.CreateInstance<CasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId>();
        }

        [Fact]
        public async Task DadoPropostaNaoEncontradaOuNaoPublicada_QuandoExecutar_EntaoDeveRetornarSucessoComDadosNulos()
        {
            // Arrange
            long propostaId = 99;
            _repositorioMock
                .Setup(r => r.ObterDetalhesPropostaConferenciaComTurmasPorIdAsync(propostaId))
                .ReturnsAsync((PropostaComTurmasDto?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().BeNull();
            _repositorioMock.Verify(r => r.ObterDetalhesPropostaConferenciaComTurmasPorIdAsync(propostaId), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaConferenciaPublicadaComTurmas_QuandoExecutar_EntaoDeveRetornarComTurmasOrdenadasNumericamente()
        {
            // Arrange
            long propostaId = 10;
            var proposta = new PropostaComTurmasDto
            {
                Id = propostaId,
                NomeFormacao = "Conferência Municipal",
                NumeroFormacao = 20261,
                Turmas =
                [
                    new PropostaTurmaDto { Id = 3, Nome = "Turma 10", PossuiCodafConferencia = true },
                    new PropostaTurmaDto { Id = 1, Nome = "Turma 2", PossuiCodafConferencia = false },
                    new PropostaTurmaDto { Id = 2, Nome = "Turma 1", PossuiCodafConferencia = false }
                ]
            };

            _repositorioMock
                .Setup(r => r.ObterDetalhesPropostaConferenciaComTurmasPorIdAsync(propostaId))
                .ReturnsAsync(proposta);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Turmas.Select(t => t.Nome).Should().ContainInOrder("Turma 1", "Turma 2", "Turma 10");
            resultado.Dados.Turmas.Last().PossuiCodafConferencia.Should().BeTrue();
        }
    }
}
