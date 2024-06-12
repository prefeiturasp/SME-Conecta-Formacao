using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExistePareceristasAdicionadosNaPropostaQuery : IRequest<bool>
    {
        public ExistePareceristasAdicionadosNaPropostaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class ExistePareceristasAdicionadosNaPropostaQueryValidator : AbstractValidator<ExistePareceristasAdicionadosNaPropostaQuery>
    {
        public ExistePareceristasAdicionadosNaPropostaQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para verificar a existência de pareceristas");
        }
    }
}
