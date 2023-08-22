using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTiposAreaPromotoraQueryHandler : IRequestHandler<ObterTiposAreaPromotoraQuery, IEnumerable<AreaPromotoraTipoDTO>>
    {
        public Task<IEnumerable<AreaPromotoraTipoDTO>> Handle(ObterTiposAreaPromotoraQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(AreaPromotoraTipo))
                .Cast<AreaPromotoraTipo>()
                .Select(v => new AreaPromotoraTipoDTO
                {
                    Id = (short)v,
                    Nome = v.Nome()
                });

            return Task.FromResult(lista);
        }
    }
}
