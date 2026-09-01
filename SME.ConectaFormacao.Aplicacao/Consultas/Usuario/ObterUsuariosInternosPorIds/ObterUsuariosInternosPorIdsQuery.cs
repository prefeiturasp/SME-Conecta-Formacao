using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterUsuariosInternosPorIdsQuery : IRequest<IEnumerable<Usuario>>
    {
        public ObterUsuariosInternosPorIdsQuery(long[] usuariosId)
        {
            UsuariosId = usuariosId;
        }

        public long[] UsuariosId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterUsuariosInternosPorIdsQueryValidator : AbstractValidator<ObterUsuariosInternosPorIdsQuery>
    {
        public ObterUsuariosInternosPorIdsQueryValidator()
        {
            RuleFor(x => x.UsuariosId).NotEmpty().WithMessage("Informe o Id do Usuario para realizar a consulta");
        }
    }
}