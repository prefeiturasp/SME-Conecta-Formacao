using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorGrupoIdQueryHandler : IRequestHandler<ObterAreaPromotoraPorGrupoIdQuery, Dominio.Entidades.AreaPromotora>
    {
        private IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreaPromotoraPorGrupoIdQueryHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public Task<Dominio.Entidades.AreaPromotora> Handle(ObterAreaPromotoraPorGrupoIdQuery request, CancellationToken cancellationToken)
        {
            return _repositorioAreaPromotora.ObterPorGrupoId(request.GrupoId);
        }
    }
}
