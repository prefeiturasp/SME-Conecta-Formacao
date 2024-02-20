using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand : IRequest
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand(long propostaId, string? registroFuncional, string? nomeRegente, long[] turmaIds)
        {
            PropostaId = propostaId;
            RegistroFuncional = registroFuncional;
            NomeRegente = nomeRegente;
            TurmaIds = turmaIds;
        }

        public long PropostaId { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? NomeRegente { get; set; }
        public long[] TurmaIds { get; set; }
    }

    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandValidator : AbstractValidator<ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand>
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta");
            RuleFor(x => x.TurmaIds).NotEmpty().WithMessage("Informe as Turmas");
        }
    }
}