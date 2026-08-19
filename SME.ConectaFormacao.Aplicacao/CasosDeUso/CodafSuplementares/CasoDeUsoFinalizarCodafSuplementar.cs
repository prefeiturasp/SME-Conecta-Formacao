using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoFinalizarCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoFinalizarCodafSuplementar
    {
        public async Task<Resultado> ExecutarAsync(long codafSuplementarId)
        {
            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codafSuplementar = await repositorioCodafSuplementar.ObterPorIdDetalhadoAsync(codafSuplementarId);

            if (codafSuplementar == null)
                return Erro.NaoEncontrado("CODAF não encontrado.");

            if (perfilRestrito && codafSuplementar.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para finalizar este CODAF.");

            if (codafSuplementar.EstaFinalizado())
                return Erro.Negocio("Não é possível finalizar um CODAF a situação 'Finalizado'.");

            if (codafSuplementar.CodafInscricoes.Count > 0 && codafSuplementar.CodafInscricoes.Any(i => i.Aprovado == true))
                return Erro.Negocio("CODAF só pode ser finalizado se não houver aprovações.");

            codafSuplementar.Finalizar();

            if (!codafSuplementar.EstaFinalizado())
                return Erro.Negocio("Não foi possível finalizar o CODAF.");

            await repositorioCodafSuplementar.Atualizar(codafSuplementar);

            return Resultado.DeSucesso();
        }
    }
}
