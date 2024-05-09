using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery : IRequest<PropostaParecerista>
    {
        public ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery(long propostaId, string registroFuncional)
        {
            PropostaId = propostaId;
            RegistroFuncional = registroFuncional;
        }

        public long PropostaId { get; }
        public string RegistroFuncional { get; }
    }

    public class ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQueryValidator : AbstractValidator<ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery>
    {
        public ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQueryValidator()
        {
            RuleFor(r => r.PropostaId)
                .NotEmpty()
                .WithMessage("Informe o Id da proposta para obter o parecerista por registro funcional");

            RuleFor(r => r.RegistroFuncional)
                .NotEmpty()
                .WithMessage("Informe o Registro Funcional do parecerista para obter");
        }
    }
}
