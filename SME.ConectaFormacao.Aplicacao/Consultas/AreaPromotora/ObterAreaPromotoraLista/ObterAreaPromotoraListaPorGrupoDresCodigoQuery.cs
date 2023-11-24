using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaPorGrupoDresCodigoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterAreaPromotoraListaPorGrupoDresCodigoQuery(Guid grupoId, IEnumerable<string> dresCodigo)
        {
            GrupoId = grupoId;
            DresCodigo = dresCodigo;
        }

        public IEnumerable<string> DresCodigo { get; set; }
        public Guid GrupoId { get; }
    }
}
