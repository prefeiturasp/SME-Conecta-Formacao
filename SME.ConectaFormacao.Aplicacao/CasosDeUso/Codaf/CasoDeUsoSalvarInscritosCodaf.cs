using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoSalvarInscritosCodaf(ICodafInscritosListaPresencaService inscritosService, IMapper mapper,
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca) : ICasoDeUsoSalvarInscritosCodaf
    {
        public async Task<Resultado> ExecutarAsync(IList<CodafInscritoListaPresencaSalvarDto> inscritos, long codafListaPresencaId)
        {
            if (!inscritos.Any())
                return Erro.Validacao("A lista de inscritos não pode ser vazia");

            var idsInscritos = inscritos.Select(i => i.InscricaoId).ToList();
            if (idsInscritos.Distinct().Count() != idsInscritos.Count)
                return Erro.Validacao("Há inscritos duplicados na lista!");

            var codaf = await repositorioCodafListaPresenca.ObterNaoExcluidosPorIdAsync(codafListaPresencaId);
            if (codaf is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (codaf.EstaFinalizado())
                return Erro.Negocio("Não é possível salvar inscritos em uma lista de presença com situação 'Finalizado'.");


            var inscritosEntidade = mapper.Map<List<CodafInscricaoListaPresenca>>(inscritos);
            await inscritosService.SalvarInscritosAsync(inscritosEntidade, codafListaPresencaId);

            return Resultado.DeSucesso();
        }
    }
}
