using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTipoInscricaoQueryHandler : IRequestHandler<ObterTipoInscricaoQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterTipoInscricaoQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(TipoInscricao))
                .Cast<TipoFormacao>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            return Task.FromResult(lista);
        }
    }
}
