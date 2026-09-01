using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterGrupoPorIdQuery : IRequest<GrupoDTO>
    {
        public ObterGrupoPorIdQuery(Guid grupoId)
        {
            GrupoId = grupoId;
        }

        public Guid GrupoId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterGrupoPorIdQueryValidator : AbstractValidator<ObterGrupoPorIdQuery>
    {
        public ObterGrupoPorIdQueryValidator()
        {
            RuleFor(x => x.GrupoId).NotEmpty().WithMessage("Informe o Id do Grupo");
        }
    }
}