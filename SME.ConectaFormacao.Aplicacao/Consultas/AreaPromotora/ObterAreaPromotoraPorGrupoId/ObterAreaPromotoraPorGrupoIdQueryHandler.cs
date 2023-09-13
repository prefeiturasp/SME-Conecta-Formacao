using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorGrupoIdQueryHandler : IRequestHandler<ObterAreaPromotoraPorGrupoIdQuery, Dominio.Entidades.AreaPromotora>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreaPromotoraPorGrupoIdQueryHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<Dominio.Entidades.AreaPromotora> Handle(ObterAreaPromotoraPorGrupoIdQuery request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorGrupoId(request.GrupoId);
            if (areaPromotora != null)
                areaPromotora.Telefones = await _repositorioAreaPromotora.ObterTelefonesPorId(areaPromotora.Id);

            return areaPromotora;
        }
    }
}
