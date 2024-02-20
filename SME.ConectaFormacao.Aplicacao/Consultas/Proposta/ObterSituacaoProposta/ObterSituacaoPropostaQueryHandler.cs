using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterSituacaoPropostaQueryHandler : IRequestHandler<ObterSituacaoPropostaQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterSituacaoPropostaQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(SituacaoProposta))
               .Cast<SituacaoProposta>()
               .Select(v => new RetornoListagemDTO
               {
                   Id = (short)v,
                   Descricao = v.Nome()
               });

            return Task.FromResult(lista);
        }
    }
}
