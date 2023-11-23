using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaRegenteCommand : IRequest<long>
    {
        public SalvarPropostaRegenteCommand(long propostaId, PropostaRegenteDTO propostaRegenteDto)
        {
            PropostaId = propostaId;
            PropostaRegenteDTO = propostaRegenteDto;
        }

        public long PropostaId { get; }
        public PropostaRegenteDTO PropostaRegenteDTO { get; }

    }
    public class SalvarPropostaRegenteCommandValidator : AbstractValidator<SalvarPropostaRegenteCommand>
    {
        public SalvarPropostaRegenteCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar o regente");
            RuleFor(x => x.PropostaRegenteDTO.NomeRegente)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1).WithMessage("Informe o nome do Regente");

            RuleFor(x => x.PropostaRegenteDTO.Turmas)
                .Must(x => x.Any()).WithMessage("É necessário informar uma Turma para cadastrar um regente");
        }
    }
}