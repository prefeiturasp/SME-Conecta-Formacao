using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosInternosPorIdsQuery : IRequest<IEnumerable<Usuario>>
    {
        public ObterUsuariosInternosPorIdsQuery(long[] usuariosId)
        {
            UsuariosId = usuariosId;
        }

        public long[] UsuariosId { get; set; }
    }

    public class ObterUsuariosInternosPorIdsQueryValidator : AbstractValidator<ObterUsuariosInternosPorIdsQuery>
    {
        public ObterUsuariosInternosPorIdsQueryValidator()
        {
            RuleFor(x => x.UsuariosId).NotEmpty().WithMessage("Informe o Id do Usuario para realizar a consulta");
        }
    }
}