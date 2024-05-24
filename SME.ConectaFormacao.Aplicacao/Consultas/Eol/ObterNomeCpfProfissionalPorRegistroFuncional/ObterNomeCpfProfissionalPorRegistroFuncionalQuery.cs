using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeCpfProfissionalPorRegistroFuncionalQuery : IRequest<RetornoUsuarioDTO>
    {
        public ObterNomeCpfProfissionalPorRegistroFuncionalQuery(string registroFuncional)
        {
            RegistroFuncional = registroFuncional;
        }

        public string RegistroFuncional { get; set; }
    }
    public class ObterNomeProfissionalPorRegistroFuncionalQueryValidator : AbstractValidator<ObterNomeCpfProfissionalPorRegistroFuncionalQuery>
    {
        public ObterNomeProfissionalPorRegistroFuncionalQueryValidator()
        {
            RuleFor(x => x.RegistroFuncional)
                .NotEmpty()
                .WithMessage(MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO_RF_INVALIDO);
        }
    }
}