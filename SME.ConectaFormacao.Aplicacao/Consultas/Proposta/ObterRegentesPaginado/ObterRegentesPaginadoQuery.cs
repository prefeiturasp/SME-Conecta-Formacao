using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Consultas
{
    public class ObterRegentesPaginadoQuery : IRequest<PaginacaoResultadoDTO<PropostaRegenteDTO>>
    {
        public ObterRegentesPaginadoQuery(long propostaId, int numeroPagina, int numeroRegistros)
        {
            PropostaId = propostaId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long PropostaId { get; set; }
        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
    }
    public class ObterRegentesPaginadoQueryValidator : AbstractValidator<ObterRegentesPaginadoQuery>
    {
        public ObterRegentesPaginadoQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter os encontros paginados");
        }
    }
}