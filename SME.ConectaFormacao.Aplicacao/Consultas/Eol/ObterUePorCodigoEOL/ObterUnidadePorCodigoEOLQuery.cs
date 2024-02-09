using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUnidadePorCodigoEOLQuery : IRequest<UnidadeEol>
    {
        public ObterUnidadePorCodigoEOLQuery(string ueCodigo)
        {
            UeCodigo = ueCodigo;
        }

        public string UeCodigo { get; set; }
    }

    public class ObterUePorCodigoEOLQueryValidator : AbstractValidator<ObterUnidadePorCodigoEOLQuery>
    {
        public ObterUePorCodigoEOLQueryValidator()
        {
            RuleFor(x => x.UeCodigo).NotEmpty().NotNull().WithMessage("Informe o código da UE para realizar a consulta no EOL");
        }
    }
}