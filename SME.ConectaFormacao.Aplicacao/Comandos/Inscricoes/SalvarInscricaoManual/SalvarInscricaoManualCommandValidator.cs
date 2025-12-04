using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoManual
{
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
