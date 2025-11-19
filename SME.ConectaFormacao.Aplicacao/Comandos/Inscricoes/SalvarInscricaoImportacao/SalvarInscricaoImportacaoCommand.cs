using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoImportacaoCommand : IRequest<bool>
    {
        public SalvarInscricaoImportacaoCommand(Inscricao inscricao, bool formacaoHomologada)
        {
            Inscricao = inscricao;
            FormacaoHomologada = formacaoHomologada;
        }

        public Inscricao Inscricao { get; }
        public bool FormacaoHomologada { get; }
    }

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
