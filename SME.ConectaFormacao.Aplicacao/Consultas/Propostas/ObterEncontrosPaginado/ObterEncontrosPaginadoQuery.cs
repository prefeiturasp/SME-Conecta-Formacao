using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterEncontrosPaginado
{
    public class ObterEncontrosPaginadoQuery(long propostaId, int numeroPagina, int numeroRegistros) : 
        IRequest<PaginacaoResultadoDto<PropostaEncontroDto>>
    {
        public long PropostaId { get; set; } = propostaId;
        public int NumeroPagina { get; } = numeroPagina;
        public int NumeroRegistros { get; } = numeroRegistros;
    }

    public class ObterEncontrosPaginadoQueryValidator : AbstractValidator<ObterEncontrosPaginadoQuery>
    {
        public ObterEncontrosPaginadoQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter os encontros paginados");
        }
    }
}
