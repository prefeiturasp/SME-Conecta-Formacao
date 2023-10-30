using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomeProfissionalPorRegistroFuncional
{
    public class ObterNomeProfissionalPorRegistroFuncionalQuery : IRequest<string>
    {
        public ObterNomeProfissionalPorRegistroFuncionalQuery(string registroFuncional)
        {
            RegistroFuncional = registroFuncional;
        }

        public string RegistroFuncional { get; set; }
    }
    public class ObterNomeProfissionalPorRegistroFuncionalQueryValidator : AbstractValidator<ObterNomeProfissionalPorRegistroFuncionalQuery>
    {
        public ObterNomeProfissionalPorRegistroFuncionalQueryValidator()
        {
            RuleFor(x => x.RegistroFuncional)
                .NotEmpty()
                .WithMessage(MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO_RF_INVALIDO);
        }
    }
}