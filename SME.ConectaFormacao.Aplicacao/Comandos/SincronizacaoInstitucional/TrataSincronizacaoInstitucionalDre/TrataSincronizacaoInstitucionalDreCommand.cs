using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao;

public class TrataSincronizacaoInstitucionalDreCommand : IRequest<bool>
{
    public TrataSincronizacaoInstitucionalDreCommand(DreServicoEol nomeAbreviacaoDto)
    {
        NomeAbreviacaoDto = nomeAbreviacaoDto;
    }

    public DreServicoEol NomeAbreviacaoDto { get; set; }
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