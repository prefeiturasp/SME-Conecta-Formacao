using FluentAssertions;
using SME.ConectaFormacao.Dominio.Entidades;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class PropostaEntidadeTestes
    {
        [Fact]
        public void DadoDataInscricaoFimComHoraZerada_QuandoAjustarDatasInscricao_EntaoDeveAjustarParaFinalDoDia()
        {
            var proposta = new Proposta
            {
                DataInscricaoInicio = new DateTime(2026, 10, 13, 0, 0, 0),
                DataInscricaoFim = new DateTime(2026, 10, 13, 0, 0, 0)
            };

            proposta.AjustarDatasInscricao();

            proposta.DataInscricaoFim.Should().Be(new DateTime(2026, 10, 13, 23, 59, 59, 999));
        }

        [Fact]
        public void DadoDataInscricaoFimComHoraInformada_QuandoAjustarDatasInscricao_EntaoNaoDeveAlterar()
        {
            var dataFimComHora = new DateTime(2026, 10, 13, 18, 0, 0);
            var proposta = new Proposta
            {
                DataInscricaoInicio = new DateTime(2026, 10, 13, 10, 0, 0),
                DataInscricaoFim = dataFimComHora
            };

            proposta.AjustarDatasInscricao();

            proposta.DataInscricaoFim.Should().Be(dataFimComHora);
        }

        [Fact]
        public void DadoDataInscricaoFimNula_QuandoAjustarDatasInscricao_EntaoDeveManterNulo()
        {
            var proposta = new Proposta
            {
                DataInscricaoInicio = null,
                DataInscricaoFim = null
            };

            proposta.AjustarDatasInscricao();

            proposta.DataInscricaoFim.Should().BeNull();
        }

        [Fact]
        public void DadoTurmaComPeriodoCustomizado_QuandoDentroDoPeriodoCustomizado_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var proposta = new Proposta
            {
                DataInscricaoInicio = new DateTime(2026, 10, 1),
                DataInscricaoFim = new DateTime(2026, 10, 31)
            };

            var grupo = new PropostaGrupoPeriodoInscricao
            {
                Id = 1,
                DataInicio = new DateTime(2026, 10, 5),
                DataFim = new DateTime(2026, 10, 15)
            };
            grupo.AdicionarTurma(100);

            // Act & Assert
            proposta.TurmaEstaEmPeriodoDeInscricao(100, new[] { grupo }, new DateTime(2026, 10, 10)).Should().BeTrue();
            proposta.TurmaEstaEmPeriodoDeInscricao(100, new[] { grupo }, new DateTime(2026, 10, 16)).Should().BeFalse();
        }

        [Fact]
        public void DadoTurmaSemPeriodoCustomizado_QuandoDentroDoPeriodoGlobal_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var proposta = new Proposta
            {
                DataInscricaoInicio = new DateTime(2026, 10, 1),
                DataInscricaoFim = new DateTime(2026, 10, 31)
            };

            // Act & Assert
            proposta.TurmaEstaEmPeriodoDeInscricao(200, null, new DateTime(2026, 10, 15)).Should().BeTrue();
            proposta.TurmaEstaEmPeriodoDeInscricao(200, Enumerable.Empty<PropostaGrupoPeriodoInscricao>(), new DateTime(2026, 11, 1)).Should().BeFalse();
        }

        [Fact]
        public void DadoTurmaComVinculoExcluido_QuandoConsultar_EntaoDeveUsarPeriodoGlobal()
        {
            // Arrange
            var proposta = new Proposta
            {
                DataInscricaoInicio = new DateTime(2026, 10, 1),
                DataInscricaoFim = new DateTime(2026, 10, 31)
            };

            var grupo = new PropostaGrupoPeriodoInscricao
            {
                Id = 1,
                DataInicio = new DateTime(2026, 10, 5),
                DataFim = new DateTime(2026, 10, 10)
            };
            grupo.AdicionarTurma(100);
            grupo.TurmasVinculadas.First().Excluido = true;

            // Act & Assert (data 2026-10-20 está fora do grupo, mas dentro do global)
            proposta.TurmaEstaEmPeriodoDeInscricao(100, new[] { grupo }, new DateTime(2026, 10, 20)).Should().BeTrue();
        }
    }
}
