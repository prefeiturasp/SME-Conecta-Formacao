using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaTutorPaginacao : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterPropostaTutorPaginacao
    {
        public CasoDeUsoObterPropostaTutorPaginacao(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<PropostaTutorDTO>> Executar(long id)
        {
            if (id == 0) return new PaginacaoResultadoDTO<PropostaTutorDTO>(new List<PropostaTutorDTO>(), 0, 0);

            return await mediator.Send(new ObterTutorPaginadoQuery(id, NumeroPagina, NumeroRegistros));
        }
    }
}