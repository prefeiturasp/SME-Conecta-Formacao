using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerPareceristaCommand : IRequest<bool>
    {
        public EnviarParecerPareceristaCommand(long propostaId, string registroFuncional, SituacaoParecerista situacao, string justificativa)
        {
            PropostaId = propostaId;
            RegistroFuncional = registroFuncional;
            Situacao = situacao;
            Justificativa = justificativa;
        }

        public long PropostaId { get; }
        public string RegistroFuncional { get; }
        public SituacaoParecerista Situacao { get; }
        public string Justificativa { get; }
    }

    public class EnviarParecerPareceristasCommandValidator : AbstractValidator<EnviarParecerPareceristaCommand>
    {
        public EnviarParecerPareceristasCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para enviar parecer do parecerista");

            RuleFor(x => x.RegistroFuncional)
                .NotEmpty()
                .WithMessage("Informe o Registro Funcional do parecerista para enviar parecer do parecerista");

            RuleFor(x => x.Justificativa)
                .MaximumLength(1000)
                .WithMessage("A Justificativa não pode conter mais que 1000 caracteres");
        }
    }
}
