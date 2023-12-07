using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand : IRequest
    {
        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand(long propostaId, string? registroFuncional, string? nomeTutor, long[] turmaIds)
        {
            PropostaId = propostaId;
            RegistroFuncional = registroFuncional;
            NomeTutor = nomeTutor;
            TurmaIds = turmaIds;
        }

        public long PropostaId { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeTutor { get; set; }
        public long[] TurmaIds { get; set; }
    }
    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandValidator : AbstractValidator<ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand>
    {
        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta");
            RuleFor(x => x.TurmaIds).NotEmpty().WithMessage("Informe as Turmas");
        }
    }
}