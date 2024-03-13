using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorIdQueryHandler : IRequestHandler<ObterAreaPromotoraPorIdQuery, AreaPromotora>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreaPromotoraPorIdQueryHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<AreaPromotora> Handle(ObterAreaPromotoraPorIdQuery request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.Id);
            if (areaPromotora != null)
                areaPromotora.Telefones = await _repositorioAreaPromotora.ObterTelefonesPorId(areaPromotora.Id);

            return areaPromotora;
        }
    }
}
