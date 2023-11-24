using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterAreaPromotoraListaQuery(Guid grupoId, IEnumerable<string> dresCodigo)
        {
            GrupoId = grupoId;
            DresCodigo = dresCodigo;
        }

        public IEnumerable<string> DresCodigo { get; set; }
        public Guid GrupoId { get; }
    }
}
