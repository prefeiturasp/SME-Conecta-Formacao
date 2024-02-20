using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUnidadePorCodigoEOLQuery : IRequest<UnidadeEol>
    {
        public ObterUnidadePorCodigoEOLQuery(string unidadeCodigo)
        {
            UnidadeCodigo = unidadeCodigo;
        }

        public string UnidadeCodigo { get; set; }
    }

    public class ObterUePorCodigoEOLQueryValidator : AbstractValidator<ObterUnidadePorCodigoEOLQuery>
    {
        public ObterUePorCodigoEOLQueryValidator()
        {
            RuleFor(x => x.UnidadeCodigo).NotEmpty().NotNull().WithMessage("Informe o código da Unidade para realizar a consulta no EOL");
        }
    }
}