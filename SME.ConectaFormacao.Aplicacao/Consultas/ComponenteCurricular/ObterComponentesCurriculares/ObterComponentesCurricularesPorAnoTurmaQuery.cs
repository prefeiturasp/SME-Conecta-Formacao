using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesPorAnoTurmaQuery : IRequest<IEnumerable<RetornoListagemTodosDTO>>
    {
        public ObterComponentesCurricularesPorAnoTurmaQuery(long[] anoTurmaId, bool exibirOpcaoTodos)
        {
            AnoTurmaId = anoTurmaId;
            ExibirOpcaoTodos = exibirOpcaoTodos;
        }

        public long[] AnoTurmaId { get; }
        public bool ExibirOpcaoTodos { get; set; }

    }

    public class ObterComponentesCurricularesPorAnoTurmaQueryValidator : AbstractValidator<ObterComponentesCurricularesPorAnoTurmaQuery>
    {
        public ObterComponentesCurricularesPorAnoTurmaQueryValidator()
        {            
            RuleFor(x => x.AnoTurmaId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador de ano da turma para obter os componentes curriculares e os Anos das Turmas");
        }
    }
}
