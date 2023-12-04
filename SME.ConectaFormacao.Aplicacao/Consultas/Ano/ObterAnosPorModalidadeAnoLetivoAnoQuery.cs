using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAnosPorModalidadeAnoLetivoAnoQuery : IRequest<IEnumerable<IdNomeOutrosDTO>>
    {
        public ObterAnosPorModalidadeAnoLetivoAnoQuery(Modalidade modalidade, int anoLetivo)
        {
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
        }

        public Modalidade Modalidade { get; }
        public int AnoLetivo { get; }
    }

    public class ObterAnoPorModalidadeAnoLetivoAnoQueryValidator : AbstractValidator<ObterAnosPorModalidadeAnoLetivoAnoQuery>
    {
        public ObterAnoPorModalidadeAnoLetivoAnoQueryValidator()
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
