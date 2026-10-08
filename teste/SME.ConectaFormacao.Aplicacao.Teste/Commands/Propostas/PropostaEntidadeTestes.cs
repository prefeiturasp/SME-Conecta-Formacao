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
    }
}
