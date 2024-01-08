using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterInscricaoPorId : CasoDeUsoAbstrato, ICasoDeUsoObterInscricaoPorId
    {
        public CasoDeUsoObterInscricaoPorId(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<DadosListagemInscricaoDTO>> Executar(long inscricaoId, FiltroListagemInscricaoDTO filtroListagemInscricaoDTO)
        {
            var dadosInscricao = await mediator.Send(new ObterInscricaoPorIdQuery(inscricaoId, filtroListagemInscricaoDTO));

            if(!dadosInscricao.Any())
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            return dadosInscricao;
        }
    }
}