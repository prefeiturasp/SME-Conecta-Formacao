using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoSalvarInscritosCodaf(ICodafInscritosListaPresencaService inscritosService, IMapper mapper,
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca, IContextoAplicacao contextoAplicacao) : ICasoDeUsoSalvarInscritosCodaf
    {
        public async Task<Resultado> ExecutarAsync(IList<CodafInscritoListaPresencaSalvarDto> inscritos, long codafListaPresencaId)
        {
            bool perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            if (!inscritos.Any())
                return Erro.Validacao("A lista de inscritos não pode ser vazia");

            var idsInscritos = inscritos.Select(i => i.InscricaoId).ToList();
            if (idsInscritos.Distinct().Count() != idsInscritos.Count)
                return Erro.Validacao("Há inscritos duplicados na lista!");

            var codaf = await repositorioCodafListaPresenca.ObterNaoExcluidosPorIdAsync(codafListaPresencaId);
            if (codaf is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (perfilRestrito && codaf.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para salvar inscritos nesta lista de presença.");

            if (codaf.EstaFinalizado())
                return Erro.Negocio("Não é possível salvar inscritos em uma lista de presença com situação 'Finalizado'.");


            var inscritosEntidade = mapper.Map<List<CodafInscricaoListaPresenca>>(inscritos);
            await inscritosService.SalvarInscritosAsync(inscritosEntidade, codafListaPresencaId);

            return Resultado.DeSucesso();
        }
    }
}
