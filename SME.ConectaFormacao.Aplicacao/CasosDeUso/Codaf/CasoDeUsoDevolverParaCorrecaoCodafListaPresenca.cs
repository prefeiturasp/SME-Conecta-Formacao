using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoDevolverParaCorrecaoCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioComentarioCodafListaPresenca repositorioComentarioCodafListaPresenca,
        ITransacao transacao) :
        ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafListaPresencaId, string justificativa)
        {
            if (codafListaPresencaId <= 0)
                return Erro.Validacao("O Id da lista de presença Codaf deve ser informado.");
            if (string.IsNullOrWhiteSpace(justificativa))
                return Erro.Validacao("A justificativa para devolução da lista de presença Codaf deve ser informada.");

            var codafListaPresenca = await repositorioCodafListaPresenca.ObterPorId(codafListaPresencaId);
            if (codafListaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença Codaf não encontrada para o Id informado.");

            if (!codafListaPresenca.PodeSerDevolvidaParaCorrecao())
                return Erro.Validacao("A lista de presença Codaf deve estar com status 'Enviada para DF' para ser devolvida para correção.");

            using var transacaoDb = transacao.Iniciar();
            try
            {
                codafListaPresenca.MarcarComoDevolvidaParaCorrecao();
                await repositorioCodafListaPresenca.Atualizar(codafListaPresenca);
                var comentario = new CodafComentarioListaPresenca { Comentario = justificativa, CodafListaPresencaId = codafListaPresenca.Id };
                await repositorioComentarioCodafListaPresenca.Inserir(comentario);
                transacaoDb.Commit();
                return true;
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Ocorreu um erro ao devolver a lista de presença Codaf para a área promotora.");
            }
        }
    }
}
