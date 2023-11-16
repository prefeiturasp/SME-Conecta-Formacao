using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeExisteRegenteTutorCommand : IRequest<IEnumerable<string>>
    {
        public ValidarSeExisteRegenteTutorCommand(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }

    }
    public class ValidarSeExisteRegenteTutorCommandValidator : AbstractValidator<ValidarSeExisteRegenteTutorCommand>
    {
        public ValidarSeExisteRegenteTutorCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta para realizar as valições");
        }
    }
}