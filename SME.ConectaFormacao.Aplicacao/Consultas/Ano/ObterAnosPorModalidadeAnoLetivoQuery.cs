using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAnosPorModalidadeAnoLetivoQuery : IRequest<IEnumerable<IdNomeTodosDTO>>
    {
        public ObterAnosPorModalidadeAnoLetivoQuery(Modalidade modalidade, int anoLetivo)
        {
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
        }

        public Modalidade Modalidade { get; }
        public int AnoLetivo { get; }
    }

    public class ObterAnoPorModalidadeAnoLetivoQueryValidator : AbstractValidator<ObterAnosPorModalidadeAnoLetivoQuery>
    {
        public ObterAnoPorModalidadeAnoLetivoQueryValidator()
        {
            RuleFor(x => x.Modalidade)
                .NotEmpty()
                .WithMessage("É necessário informar a modalidade para obter os componentes curriculares");
            
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares");
        }
    }
}
