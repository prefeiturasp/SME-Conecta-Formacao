using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeExisteRegenteTutorCommand : IRequest<string>
    {
        public ValidarSeExisteRegenteTutorCommand(long propostaId,short quantidadeTurmas)
        {
            PropostaId = propostaId;
            QuantidadeTurmas = quantidadeTurmas;
        }

        public long PropostaId { get; set; }
        public short QuantidadeTurmas { get; set; }

    }
    public class ValidarSeExisteRegenteTutorCommandValidator : AbstractValidator<ValidarSeExisteRegenteTutorCommand>
    {
        public ValidarSeExisteRegenteTutorCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta para realizar as valições");
        }
    }
}