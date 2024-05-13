using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPareceristasAdicionadosNaPropostaQuery : IRequest<IEnumerable<PropostaParecerista>> 
    {
        public ObterPareceristasAdicionadosNaPropostaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class ObterPareceristasAdicionadosNaPropostaQueryValidator : AbstractValidator<ObterPareceristasAdicionadosNaPropostaQuery>
    {
        public ObterPareceristasAdicionadosNaPropostaQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para retornar a existência de pareceristas");
        }
    }
}
