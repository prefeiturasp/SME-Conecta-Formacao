using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand : IRequest<bool>
{
    public TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand(Ano ano, ComponenteCurricular componenteCurricular, bool mudouAnoTurma = true, bool mudouComponente = true)
    {
        ComponenteCurricular = componenteCurricular;
        Ano = ano;
        MudouAnoTurma  = mudouAnoTurma;
        MudouComponente = mudouComponente;
    }

    public bool MudouAnoTurma { get; set; }
    public bool MudouComponente { get; set; }
    public Ano Ano { get; set; }
    public ComponenteCurricular ComponenteCurricular { get; set; }

    public class TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommandValidator : AbstractValidator<TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand>
    {
        public TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommandValidator()
        {
            RuleFor(f => f.Ano)
                .NotNull()
                .WithMessage("É necessário informar o ano para a realizar a sincronização de Anos e Componentes Curriculares");
            
            RuleFor(f => f.ComponenteCurricular)
                .NotNull()
                .WithMessage("É necessário informar o componente curricular para a realizar a sincronização de Anos e Componentes Curriculares");
        }
    }
}