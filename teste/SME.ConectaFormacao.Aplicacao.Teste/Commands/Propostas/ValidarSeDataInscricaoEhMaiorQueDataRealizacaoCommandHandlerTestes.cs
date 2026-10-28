using FluentAssertions;
using SME.ConectaFormacao.Dominio.Constantes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommandHandlerTestes
    {
        private readonly ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommandHandler _sut = new();

        [Fact]
        public async Task DadoMesmoDiaComHoraFimInscricao23h59EDataRealizacaoSemHora_QuandoValidar_EntaoNaoDeveRetornarErro()
        {
            // DataRealizacaoFim sem hora (00:00:00) e DataInscricaoFim às 23:59:59.999 no mesmo dia
            var dataRealizacaoFim = new DateTime(2026, 10, 13, 0, 0, 0);
            var dataInscricaoFim = new DateTime(2026, 10, 13, 23, 59, 59, 999);

            var comando = new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(dataInscricaoFim, dataRealizacaoFim);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoDataInscricaoFimDiaSeguinte_QuandoValidar_EntaoDeveRetornarErro()
        {
            var dataRealizacaoFim = new DateTime(2026, 10, 13, 0, 0, 0);
            var dataInscricaoFim = new DateTime(2026, 10, 14, 0, 0, 0);

            var comando = new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(dataInscricaoFim, dataRealizacaoFim);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().Be(MensagemNegocio.DATAFIM_INSCRICAO_NAO_PODE_SER_MAIOR_QUE_DATAFIM_REALIZACAO);
        }

        [Fact]
        public async Task DadoDataInscricaoFimAnterior_QuandoValidar_EntaoNaoDeveRetornarErro()
        {
            var dataRealizacaoFim = new DateTime(2026, 10, 20, 0, 0, 0);
            var dataInscricaoFim = new DateTime(2026, 10, 15, 18, 0, 0);

            var comando = new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(dataInscricaoFim, dataRealizacaoFim);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoDatasNulas_QuandoValidar_EntaoNaoDeveRetornarErro()
        {
            var comando = new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(null, null);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().BeEmpty();
        }
    }
}
