using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular
{
    public interface ICasoDeUsoObterAnosPorModalidadeAnoLetivo
    {
        Task<IEnumerable<IdNomeOutrosDTO>> Executar(ModalidadeAnoLetivoFiltrosDTO modalidadeAnoLetivoFiltrosDto);
    }
}
