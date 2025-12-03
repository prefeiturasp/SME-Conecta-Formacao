using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUnidadePorCodigoEOLQuery(string? unidadeCodigo) : IRequest<UnidadeEol>
    {
        public string? UnidadeCodigo { get; set; } = unidadeCodigo;
    }

    public class ObterUePorCodigoEOLQueryValidator : AbstractValidator<ObterUnidadePorCodigoEOLQuery>
    {
        public ObterUePorCodigoEOLQueryValidator()
        {
            RuleFor(x => x.UnidadeCodigo).NotEmpty().NotNull().WithMessage("Informe o código da Unidade para realizar a consulta no EOL");
        }
    }
}