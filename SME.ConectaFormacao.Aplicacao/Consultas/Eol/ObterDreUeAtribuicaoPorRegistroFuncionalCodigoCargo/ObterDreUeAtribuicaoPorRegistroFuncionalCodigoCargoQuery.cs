using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery : IRequest<IEnumerable<DreUeAtribuicaoServicoEol>>
    {
        public ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery(string registroFuncional, string codigoCargo)
        {
            RegistroFuncional = registroFuncional;
            CodigoCargo = codigoCargo;
        }

        public string RegistroFuncional { get; }
        public string CodigoCargo { get; }
    }

    public class ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryValidator : AbstractValidator<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery>
    {
        public ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryValidator()
        {
            RuleFor(t => t.RegistroFuncional)
                .NotEmpty()
                .WithMessage("O registro funcional deve ser informado para consultar as dres e ues atribuidos ao funcionario");

            RuleFor(t => t.CodigoCargo)
                .NotEmpty()
                .WithMessage("O código do cargo deve ser informado para consultar as dres e ues atribuidos ao funcionario");
        }
    }
}
