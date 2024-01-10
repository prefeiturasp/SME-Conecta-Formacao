using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQuery : IRequest<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>>
    {
        public ObterInscricaoPorIdQuery(long inscricaoId, FiltroListagemInscricaoDTO filtroListagemInscricaoDto, int numeroPagina, int numeroRegistros)
        {
            InscricaoId = inscricaoId;
            filtros = filtroListagemInscricaoDto;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long InscricaoId { get; set; }
        public FiltroListagemInscricaoDTO filtros { get; set; }
        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
    }

    class ObterInscricaoPorIdQueryValidator : AbstractValidator<ObterInscricaoPorIdQuery>
    {
        public ObterInscricaoPorIdQueryValidator()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0).WithMessage("Informe o Id da Inscrição para realizar a consulta");
        }
    }
}