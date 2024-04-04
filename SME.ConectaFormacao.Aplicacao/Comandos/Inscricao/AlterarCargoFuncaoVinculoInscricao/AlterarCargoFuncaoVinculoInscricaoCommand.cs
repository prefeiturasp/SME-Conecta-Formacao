using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarCargoFuncaoVinculoInscricaoCommand : IRequest<bool>
    {
        public AlterarCargoFuncaoVinculoInscricaoCommand(long id, AlterarCargoFuncaoVinculoIncricaoDTO alterarCargoFuncaoVinculoIncricao)
        {
            Id = id;
            AlterarCargoFuncaoVinculoIncricao = alterarCargoFuncaoVinculoIncricao;
        }

        public long Id { get; }
        public AlterarCargoFuncaoVinculoIncricaoDTO AlterarCargoFuncaoVinculoIncricao { get; }
    }

    public class AlterarCargoFuncaoVinculoInscricaoCommandValidator : AbstractValidator<AlterarCargoFuncaoVinculoInscricaoCommand>
    {
        public AlterarCargoFuncaoVinculoInscricaoCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");

            RuleFor(c => c.AlterarCargoFuncaoVinculoIncricao)
                .NotNull()
                .WithMessage("Os dados para alteração do cargo, função e vínculo da inscrição devem ser informados.")
                .DependentRules(() =>
                {
                    RuleFor(c => c.AlterarCargoFuncaoVinculoIncricao.CargoCodigo)
                        .NotEmpty()
                        .NotNull()
                        .WithMessage("É necessário informar o cargo da inscrição");

                    RuleFor(c => c.AlterarCargoFuncaoVinculoIncricao.TipoVinculo)
                        .GreaterThan(0)
                        .WithMessage("É necessário informa o vínculo da inscrição.");
                });
        }
    }
}