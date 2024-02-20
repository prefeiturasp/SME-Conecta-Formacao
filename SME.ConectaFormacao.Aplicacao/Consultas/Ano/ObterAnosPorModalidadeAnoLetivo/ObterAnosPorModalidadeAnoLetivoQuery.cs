using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAnosPorModalidadeAnoLetivoQuery : IRequest<IEnumerable<RetornoListagemTodosDTO>>
    {
        public ObterAnosPorModalidadeAnoLetivoQuery(Modalidade[] modalidade, int anoLetivo, bool exibirTodos)
        {
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
            ExibirTodos = exibirTodos;
        }

        public Modalidade[] Modalidade { get; }
        public int AnoLetivo { get; }
        public bool ExibirTodos { get; set; }
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
