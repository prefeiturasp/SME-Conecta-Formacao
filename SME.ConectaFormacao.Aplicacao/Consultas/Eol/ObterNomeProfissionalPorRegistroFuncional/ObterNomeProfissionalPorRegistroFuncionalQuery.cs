using FluentValidation;
using MediatR;

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
                .WithMessage("É necessário informar um RF válido para obter o nome do profissional");
        }
    }
}