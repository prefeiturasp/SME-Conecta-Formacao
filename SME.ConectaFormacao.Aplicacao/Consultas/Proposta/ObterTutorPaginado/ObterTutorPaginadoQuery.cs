using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTutorPaginadoQuery : IRequest<PaginacaoResultadoDTO<PropostaTutorDTO>>
    {
        public ObterTutorPaginadoQuery(long propostaId, int numeroPagina, int numeroRegistros)
        {
            PropostaId = propostaId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long PropostaId { get; set; }
        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }

        public class ObterTutorPaginadoQueryValidator : AbstractValidator<ObterTutorPaginadoQuery>
        {
            public ObterTutorPaginadoQueryValidator()
            {
                RuleFor(x => x.PropostaId)
                    .NotEmpty()
                    .WithMessage("É necessário informar o id da proposta para obter os encontros paginados");
            }
        }
    }
}