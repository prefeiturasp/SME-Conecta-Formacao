using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarGrupoAreaPromotoraCommandHandler : IRequestHandler<ValidarGrupoAreaPromotoraCommand>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ValidarGrupoAreaPromotoraCommandHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task Handle(ValidarGrupoAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            if (await _repositorioAreaPromotora.ExistePorGrupoId(request.GrupoId, request.IgnorarAreaPromotoraId))
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_EXISTE_GRUPO_CADASTRADO);
        }
    }
}
