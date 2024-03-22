using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarVinculoInscricaoCommand : IRequest<bool>
    {
        public AlterarVinculoInscricaoCommand(long id, int tipoVinculo)
        {
            Id = id;
            TipoVinculo = tipoVinculo;
        }

        public long Id { get; }
        public int TipoVinculo { get; }
    }

    public class AlterarVinculoInscricaoCommandValidator : AbstractValidator<AlterarVinculoInscricaoCommand>
    {
        public AlterarVinculoInscricaoCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");
            
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o vínculo da inscrição");            
        }
    }
}