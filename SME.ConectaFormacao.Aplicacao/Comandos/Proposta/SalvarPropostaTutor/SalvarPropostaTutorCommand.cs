using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Proposta.SalvarPropostaTutor
{
    public class SalvarPropostaTutorCommand : IRequest<long>
    {
        public SalvarPropostaTutorCommand(long propostaId, PropostaTutorDTO propostaTutorDto)
        {
            PropostaId = propostaId;
            PropostaTutorDto = propostaTutorDto;
        }

        public long PropostaId { get; }
        public PropostaTutorDTO PropostaTutorDto { get; }
    }
    public class SalvarPropostaTutorCommandValidator : AbstractValidator<SalvarPropostaTutorCommand>
    {
        public SalvarPropostaTutorCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar o tutor");
            RuleFor(x => x.PropostaTutorDto.NomeTutor)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1).WithMessage("Informe o nome do Tutor");
            
            RuleFor(x => x.PropostaTutorDto.Turmas)
                .Must(x => x.Any()).WithMessage("É necessário informar uma Turma para para cadastrar um tutor");
        }
    }
}