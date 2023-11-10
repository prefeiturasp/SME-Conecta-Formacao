using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterGrupoPorIdQuery : IRequest<GrupoDTO>
    {
        public ObterGrupoPorIdQuery(Guid grupoId)
        {
            GrupoId = grupoId;
        }

        public Guid GrupoId { get; set; }
    }

    public class ObterGrupoPorIdQueryValidator : AbstractValidator<ObterGrupoPorIdQuery>
    {
        public ObterGrupoPorIdQueryValidator()
        {
            RuleFor(x => x.GrupoId).NotEmpty().WithMessage("Informe o Id do Grupo");
        }
    }
}