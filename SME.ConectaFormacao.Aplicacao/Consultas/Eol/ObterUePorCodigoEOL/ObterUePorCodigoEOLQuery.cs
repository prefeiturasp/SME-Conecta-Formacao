using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUePorCodigoEOLQuery : IRequest<UeServicoEol>
    {
        public ObterUePorCodigoEOLQuery(string ueCodigo)
        {
            UeCodigo = ueCodigo;
        }

        public string UeCodigo { get; set; }
    }

    public class ObterUePorCodigoEOLQueryValidator :AbstractValidator<ObterUePorCodigoEOLQuery>
    {
        public ObterUePorCodigoEOLQueryValidator()
        {
            RuleFor(x => x.UeCodigo).NotEmpty().NotNull().WithMessage("Informe o código da UE para realizar a consulta no EOL");
        }
    }
}