using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand : IRequest<bool>
    {
        public AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(long id, IEnumerable<DadosInscricaoCargoEol> dadosInscricao)
        {
            Id = id;
            DadosInscricao = dadosInscricao;
        }

        public long Id { get; }

        public IEnumerable<DadosInscricaoCargoEol> DadosInscricao { get; }
    }


    [ExcludeFromCodeCoverage]
    public class AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandValidator : AbstractValidator<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>
    {
        public AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandValidator()
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