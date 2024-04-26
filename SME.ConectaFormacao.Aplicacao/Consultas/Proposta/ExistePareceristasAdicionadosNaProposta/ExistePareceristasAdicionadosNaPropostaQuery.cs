using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExistePareceristasAdicionadosNaPropostaQuery : IRequest<bool> 
    {
        public ExistePareceristasAdicionadosNaPropostaQuery(long prospotaId)
        {
            ProspotaId = prospotaId;
        }

        public long ProspotaId { get; set; }
    }

    public class ExistePareceristasAdicionadosNaPropostaQueryValidator : AbstractValidator<ExistePareceristasAdicionadosNaPropostaQuery>
    {
        public ExistePareceristasAdicionadosNaPropostaQueryValidator()
        {
            RuleFor(x => x.ProspotaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para verificar a existência de pareceristas");
        }
    }
}
