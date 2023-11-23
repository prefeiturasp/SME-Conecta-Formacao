using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPerfilDreAreaPromotoraCommandHandler : IRequestHandler<ValidarPerfilDreAreaPromotoraCommand>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ValidarPerfilDreAreaPromotoraCommandHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task Handle(ValidarPerfilDreAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            if (await _repositorioAreaPromotora.ExistePorGrupoIdEDreId(request.DreId, request.GrupoId, request.IgnorarAreaPromotoraId))
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_EXISTE_GRUPO_DRE_CADASTRADO);
        }
    }
}