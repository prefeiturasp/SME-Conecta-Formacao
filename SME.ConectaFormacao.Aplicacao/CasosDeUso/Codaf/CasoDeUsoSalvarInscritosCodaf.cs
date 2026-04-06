using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoSalvarInscritosCodaf(ICodafInscritosListaPresencaService inscritosService, IMapper mapper) : ICasoDeUsoSalvarInscritosCodaf
    {
        public async Task<Resultado> ExecutarAsync(IList<CodafInscritoListaPresencaSalvarDto> inscritos, long codafListaPresencaId)
        {
            if (!inscritos.Any())
                return Erro.Validacao("A lista de inscritos não pode ser vazia");

            var idsInscritos = inscritos.Select(i => i.InscricaoId).ToList();
            if (idsInscritos.Distinct().Count() != idsInscritos.Count)
                return Erro.Validacao("Há inscritos duplicados na lista!");

            var inscritosEntidade = mapper.Map<List<CodafInscricaoListaPresenca>>(inscritos);
            await inscritosService.SalvarInscritosAsync(inscritosEntidade, codafListaPresencaId);

            return Resultado.DeSucesso();
        }
    }
}
