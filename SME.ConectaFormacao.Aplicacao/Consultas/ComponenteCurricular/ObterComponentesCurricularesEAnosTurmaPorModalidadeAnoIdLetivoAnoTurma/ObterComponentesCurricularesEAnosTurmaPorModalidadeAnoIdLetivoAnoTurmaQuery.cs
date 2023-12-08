using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesEAnosTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery : IRequest<IEnumerable<RetornoListagemTodosDTO>>
    {
        public ObterComponentesCurricularesEAnosTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery(Modalidade modalidade, int anoLetivo, long anoTurmaId)
        {
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
            AnoTurmaId = anoTurmaId;
        }

        public Modalidade Modalidade { get; }
        public int AnoLetivo { get; }
        public long AnoTurmaId { get; }
    }

    public class ObterComponentesCurricularesEAnosTurmaPorModalidadeAnoLetivoAnoTurmaQueryValidator : AbstractValidator<ObterComponentesCurricularesEAnosTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery>
    {
        public ObterComponentesCurricularesEAnosTurmaPorModalidadeAnoLetivoAnoTurmaQueryValidator()
        {
            RuleFor(x => x.Modalidade)
                .NotEmpty()
                .WithMessage("É necessário informar a modalidade para obter os componentes curriculares e os Anos das Turmas");
            
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares e os Anos das Turmas");
            
            RuleFor(x => x.AnoTurmaId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador de ano da turma para obter os componentes curriculares e os Anos das Turmas");
        }
    }
}
