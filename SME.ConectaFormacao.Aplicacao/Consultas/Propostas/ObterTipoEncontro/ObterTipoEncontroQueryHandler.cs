using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTipoEncontroQueryHandler : IRequestHandler<ObterTipoEncontroQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterTipoEncontroQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(TipoEncontro))
                .Cast<TipoEncontro>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            return Task.FromResult(lista);
        }
    }
}
