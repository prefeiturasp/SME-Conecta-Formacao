using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterEncontrosPaginadoQuery : IRequest<PaginacaoResultadoDTO<PropostaEncontroDTO>>
    {
        public ObterEncontrosPaginadoQuery(long propostaId, int numeroPagina, int numeroRegistros)
        {
            PropostaId = propostaId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long PropostaId { get; set; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }

    public class ObterEncontrosPaginadoQueryValidator : AbstractValidator<ObterEncontrosPaginadoQuery>
    {
        public ObterEncontrosPaginadoQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É nescessário informar o id da proposta para obter os encontros paginados");
        }
    }
}
