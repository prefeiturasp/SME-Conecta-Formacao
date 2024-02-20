using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorGrupoIdEDresQueryHandler : IRequestHandler<ObterAreaPromotoraPorGrupoIdEDresQuery, Dominio.Entidades.AreaPromotora>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreaPromotoraPorGrupoIdEDresQueryHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<Dominio.Entidades.AreaPromotora> Handle(ObterAreaPromotoraPorGrupoIdEDresQuery request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorGrupoIdEDres(request.GrupoId, request.Dres);
            if (areaPromotora != null)
                areaPromotora.Telefones = await _repositorioAreaPromotora.ObterTelefonesPorId(areaPromotora.Id);

            return areaPromotora;
        }
    }
}
