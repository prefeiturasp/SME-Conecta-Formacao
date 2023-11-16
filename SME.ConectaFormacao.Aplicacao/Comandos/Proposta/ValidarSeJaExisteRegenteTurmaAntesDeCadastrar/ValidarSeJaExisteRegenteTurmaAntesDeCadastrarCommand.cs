using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand :IRequest
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand(long propostaId, string? registroFuncional, string? nomeRegente, int[] turmas)
        {
            PropostaId = propostaId;
            RegistroFuncional = registroFuncional;
            NomeRegente = nomeRegente;
            Turmas = turmas;
        }

        public long PropostaId { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeRegente { get; set; }
        public int[] Turmas { get; set; }
    }

    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandValidator : AbstractValidator<ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand>
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta");
            RuleFor(x => x.Turmas).NotEmpty().WithMessage("Informe as Turmas");
        }
    }
}