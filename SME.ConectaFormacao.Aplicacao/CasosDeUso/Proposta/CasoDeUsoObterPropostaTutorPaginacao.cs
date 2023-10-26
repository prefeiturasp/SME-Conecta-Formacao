using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterTutorPaginado;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaTutorPaginacao : CasoDeUsoAbstrato,ICasoDeUsoObterPropostaTutorPaginacao
    {
        public CasoDeUsoObterPropostaTutorPaginacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PaginacaoResultadoDTO<PropostaTutorDTO>> Executar(long id)
        {
            int numeroPagina = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroPagina")), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroRegistros")), out numeroRegistros) ? numeroRegistros : 10;
            if(id == 0) return new PaginacaoResultadoDTO<PropostaTutorDTO>(new List<PropostaTutorDTO>(), 0, 0);
            
            return await mediator.Send(new ObterTutorPaginadoQuery(id, numeroPagina, numeroRegistros));
        }
    }
}