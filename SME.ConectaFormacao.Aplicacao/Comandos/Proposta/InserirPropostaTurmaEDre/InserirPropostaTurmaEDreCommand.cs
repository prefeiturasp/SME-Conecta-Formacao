using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaTurmaEDreCommand : IRequest<long>
    {
        public InserirPropostaTurmaEDreCommand(PropostaTurma turma)
        {
            Turma = turma;
        }

        public PropostaTurma Turma { get; }
    }

    public class InserirPropostaTurmaEDreCommandValidator : AbstractValidator<InserirPropostaTurmaEDreCommand>
    {
        public InserirPropostaTurmaEDreCommandValidator()
        {
            RuleFor(x => x.Turma.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para inserir as turmas");

            RuleFor(x => x.Turma.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar um nome da turma da proposta para salvar as turmas");
        }
    }
}
