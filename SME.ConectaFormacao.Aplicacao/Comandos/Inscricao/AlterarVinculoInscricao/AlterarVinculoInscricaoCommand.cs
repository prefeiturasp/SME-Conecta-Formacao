using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarVinculoInscricaoCommand : IRequest<bool>
    {
        public AlterarVinculoInscricaoCommand(long id, IEnumerable<DadosInscricaoCargoEol> dadosInscricao)
        {
            Id = id;
            DadosInscricao = dadosInscricao;
        }

        public long Id { get; }

        public IEnumerable<DadosInscricaoCargoEol> DadosInscricao { get; }
    }

    public class AlterarVinculoInscricaoCommandValidator : AbstractValidator<AlterarVinculoInscricaoCommand>
    {
        public AlterarVinculoInscricaoCommandValidator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("O Id da inscrição deve ser informado para alteração do vínculo.");

            RuleFor(c => c.DadosInscricao)
                .NotNull()
                .WithMessage("Os dados da inscrição devem ser informados para a alteração.");
        }
    }
}