using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao;

[ExcludeFromCodeCoverage]
public class TrataSincronizacaoInstitucionalDreCommand : IRequest<bool>
{
    public TrataSincronizacaoInstitucionalDreCommand(DreServicoEol nomeAbreviacaoDto)
    {
        NomeAbreviacaoDto = nomeAbreviacaoDto;
    }

    public DreServicoEol NomeAbreviacaoDto { get; set; }
}

[ExcludeFromCodeCoverage]
public class TrataSincronizacaoInstitucionalDreCommandValidator : AbstractValidator<TrataSincronizacaoInstitucionalDreCommand>
{
    public TrataSincronizacaoInstitucionalDreCommandValidator()
    {
        RuleFor(c => c.NomeAbreviacaoDto)
            .NotEmpty()
            .WithMessage("A Dre deve ser informada.");
    }
}