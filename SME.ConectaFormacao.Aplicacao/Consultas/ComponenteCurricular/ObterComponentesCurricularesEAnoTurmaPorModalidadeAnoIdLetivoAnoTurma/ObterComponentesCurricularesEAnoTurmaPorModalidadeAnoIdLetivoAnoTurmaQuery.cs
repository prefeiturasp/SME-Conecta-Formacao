using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery : IRequest<IEnumerable<RetornoListagemTodosDTO>>
    {
        public ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery(Modalidade modalidade, int anoLetivo, long anoId)
        {
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
            AnoId = anoId;
        }

        public Modalidade Modalidade { get; }
        public int AnoLetivo { get; }
        public long AnoId { get; }
    }

    public class ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoLetivoAnoTurmaQueryValidator : AbstractValidator<ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery>
    {
        public ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoLetivoAnoTurmaQueryValidator()
        {
            RuleFor(x => x.Modalidade)
                .NotEmpty()
                .WithMessage("É necessário informar a modalidade para obter os componentes curriculares");
            
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares");
            
            RuleFor(x => x.AnoId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador de ano para obter os componentes curriculares");
        }
    }
}
