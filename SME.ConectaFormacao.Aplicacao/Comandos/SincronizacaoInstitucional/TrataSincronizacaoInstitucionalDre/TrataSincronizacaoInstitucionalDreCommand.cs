using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class TrataSincronizacaoInstitucionalDreCommand : IRequest<bool>
{
    public TrataSincronizacaoInstitucionalDreCommand(DreNomeAbreviacaoDTO nomeAbreviacaoDto)
    {
        NomeAbreviacaoDto = nomeAbreviacaoDto;
    }

    public DreNomeAbreviacaoDTO NomeAbreviacaoDto { get; set; }
}

public class TrataSincronizacaoInstitucionalDreCommandValidator : AbstractValidator<TrataSincronizacaoInstitucionalDreCommand>
{
    public TrataSincronizacaoInstitucionalDreCommandValidator()
    {
        RuleFor(c => c.NomeAbreviacaoDto)
            .NotEmpty()
            .WithMessage("A Dre deve ser informada.");
    }
}