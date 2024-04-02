using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQuery : IRequest<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>>
    {
        public ObterInscricaoPorIdQuery(long propostaId, FiltroListagemInscricaoDTO filtroListagemInscricaoDto, int numeroPagina, int numeroRegistros)
        {
            PropostaId = propostaId;
            filtros = filtroListagemInscricaoDto;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long PropostaId { get; set; }
        public FiltroListagemInscricaoDTO filtros { get; set; }
        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
    }

    class ObterInscricaoPorIdQueryValidator : AbstractValidator<ObterInscricaoPorIdQuery>
    {
        public ObterInscricaoPorIdQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para realizar a consulta");
        }
    }
}