using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterFormatos
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar(TipoFormacao tipoFormacao);
    }
}
