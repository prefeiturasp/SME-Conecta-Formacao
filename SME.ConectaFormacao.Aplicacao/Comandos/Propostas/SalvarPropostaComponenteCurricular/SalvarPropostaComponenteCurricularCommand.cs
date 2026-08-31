using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarPropostaComponenteCurricularCommand : IRequest<bool>
    {
        public SalvarPropostaComponenteCurricularCommand(long propostaId, IEnumerable<PropostaComponenteCurricular> componentesCurriculares)
        {
            PropostaId = propostaId;
            ComponentesCurriculares = componentesCurriculares;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaComponenteCurricular> ComponentesCurriculares { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class SalvarPropostaComponenteCurricularCommandValidator : AbstractValidator<SalvarPropostaComponenteCurricularCommand>
    {
        public SalvarPropostaComponenteCurricularCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar os componentes curriculares da proposta");
        }
    }
}
