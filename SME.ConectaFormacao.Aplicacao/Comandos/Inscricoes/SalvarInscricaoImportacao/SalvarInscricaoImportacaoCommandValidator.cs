using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao
{
    public class SalvarInscricaoImportacaoCommandValidator : AbstractValidator<SalvarInscricaoImportacaoCommand>
    {
        public SalvarInscricaoImportacaoCommandValidator()
        {
            RuleFor(x => x.Inscricao)
                .NotEmpty()
                .WithMessage("É necessário informar a inscrição para persistência de inscrições");
        }
    }
}
