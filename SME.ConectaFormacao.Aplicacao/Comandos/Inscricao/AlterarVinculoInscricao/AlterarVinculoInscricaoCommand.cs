using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarVinculoInscricaoCommand : IRequest<bool>
    {
        public AlterarVinculoInscricaoCommand(long id, VinculoIncricaoDTO vinculoIncricao)
        {
            Id = id;
            VinculoIncricao = vinculoIncricao;
        }

        public long Id { get; }
        public VinculoIncricaoDTO VinculoIncricao { get; }
    }

    public class AlterarVinculoInscricaoCommandValidator : AbstractValidator<AlterarVinculoInscricaoCommand>
    {
        public AlterarVinculoInscricaoCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");

            RuleFor(c => c.VinculoIncricao)
                .NotNull()
                .WithMessage("Os dados para alteração do vínculo da inscrição devem ser informados.")
                .DependentRules(() =>
                {
                    RuleFor(c => c.VinculoIncricao.CargoCodigo)
                        .NotEmpty()
                        .NotNull()
                        .WithMessage("É necessário informar o cargo da inscrição");

                    RuleFor(c => c.VinculoIncricao.TipoVinculo)
                        .GreaterThan(0)
                        .WithMessage("É necessário informa o vínculo da inscrição.");
                });
        }
    }
}