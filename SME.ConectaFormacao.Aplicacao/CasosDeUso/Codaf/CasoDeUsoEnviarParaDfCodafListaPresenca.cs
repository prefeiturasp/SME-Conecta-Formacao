using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoEnviarParaDfCodafListaPresenca(
        IValidadorCodafListaPresencaService validadorCodafListaPresencaService,
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca) : ICasoDeUsoEnviarParaDfCodafListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafListaPresencaId)
        {
            var codafListaPresenca = await repositorioCodafListaPresenca.ObterPorIdDetalhadoAsync(codafListaPresencaId);
            if (codafListaPresenca is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");
            if (!codafListaPresenca.PodeSerEnviadaParaDf())
                return Erro.Negocio("Lista de presença não pode ser enviada para o DF.");
            var erroValidacao = await validadorCodafListaPresencaService.ValidarParaEnvioAoDfAsync(codafListaPresenca);
            if (erroValidacao is not null)
                return erroValidacao;
            codafListaPresenca.MarcarComoEnviadaParaDf();
            await repositorioCodafListaPresenca.Atualizar(codafListaPresenca);
            return true;
        }
    }
}
