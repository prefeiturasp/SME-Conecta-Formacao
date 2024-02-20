using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoAutomaticaCommand : IRequest<long>
    {
        public SalvarInscricaoAutomaticaCommand(InscricaoAutomaticaDTO inscricaoAutomaticaDto)
        {
            InscricaoAutomaticaDTO = inscricaoAutomaticaDto;
        }

        public InscricaoAutomaticaDTO InscricaoAutomaticaDTO { get; }
    }

    public class SalvarInscricaoAutomaticaCommandValidator : AbstractValidator<SalvarInscricaoAutomaticaCommand>
    {
        public SalvarInscricaoAutomaticaCommandValidator()
        {
            RuleFor(f => f.InscricaoAutomaticaDTO.PropostaTurmaId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id da proposta turma para fazer a inscrição automática");

            RuleFor(f => f.InscricaoAutomaticaDTO.UsuarioId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id do usuário para fazer a inscrição automática");
        }
    }
}
