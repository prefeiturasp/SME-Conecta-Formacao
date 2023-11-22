using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ParecerPropostaCommand : IRequest<bool>
    {
        public ParecerPropostaCommand(long propostaId,ParecerPropostaDTO parecerPropostaDto)
        {
            PropostaId = propostaId;
            ParecerProposta = parecerPropostaDto;
        }

        public long PropostaId { get; set; }
        public ParecerPropostaDTO ParecerProposta { get; set; }

    }

    public class ParecerPropostaCommandValidator : AbstractValidator<ParecerPropostaCommand>
    {
        public ParecerPropostaCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para salvar o parecer da proposta");
            
            RuleFor(x => x.ParecerProposta.Parecer)
                .NotEmpty()
                .WithMessage("Informe o parecer para salvar o parecer da proposta");
            
            RuleFor(x => x.ParecerProposta.Situacao)
                .NotNull()
                .NotEmpty()
                .WithMessage("Informe a situação para salvar o parecer da proposta");
        }
    }
}