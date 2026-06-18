using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoObterCodafSuplementarPorCodafId(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IMapper mapper) :
        ICasoDeUsoObterCodafSuplementarPorCodafId
    {
        public async Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(long codafId)
        {
            var listaPresenca = await repositorioCodafListaPresenca.ObterPorIdDetalhadoAsync(codafId);
            if (listaPresenca == null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            var codafSuplementarDetalhadoDto = mapper.Map<CodafSuplementarDetalhadoDto>(listaPresenca);
            return codafSuplementarDetalhadoDto;
        }
    }
}
