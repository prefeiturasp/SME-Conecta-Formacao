using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoManualCommand : IRequest<RetornoDTO>
    {
        public SalvarInscricaoManualCommand(InscricaoManualDTO inscricaoManualDTO)
        {
            InscricaoManualDTO = inscricaoManualDTO;
        }

        public InscricaoManualDTO InscricaoManualDTO { get; }
    }

    public class SalvarInscricaoManualCommandValidator : AbstractValidator<SalvarInscricaoManualCommand>
    {
        public SalvarInscricaoManualCommandValidator()
        {
            RuleFor(r => r.InscricaoManualDTO.PropostaTurmaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da turma para salvar inscrição manual");

            RuleFor(r => r.InscricaoManualDTO.Cpf)
                .NotEmpty()
                .WithMessage("É necessário informar o cpf do cursista para salvar inscrição manual");
        }
    }
}
