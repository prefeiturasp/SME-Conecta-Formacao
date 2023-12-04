using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery : IRequest<IEnumerable<IdNomeOutrosDTO>>
    {
        public ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery(Modalidade modalidade, int anoLetivo, int ano)
        {
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
            Ano = ano;
        }

        public Modalidade Modalidade { get; }
        public int AnoLetivo { get; }
        public int Ano { get; }
    }

    public class ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQueryValidator : AbstractValidator<ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQuery>
    {
        public ObterComponentesCurricularesPorModalidadeAnoLetivoAnoQueryValidator()
        {
            RuleFor(x => x.Modalidade)
                .NotEmpty()
                .WithMessage("É necessário informar a modalidade para obter os componentes curriculares");
            
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares");
            
            RuleFor(x => x.Ano)
                .NotEmpty()
                .WithMessage("É necessário informar o ano para obter os componentes curriculares");
        }
    }
}
