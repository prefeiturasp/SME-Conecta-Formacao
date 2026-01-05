using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterRegistrosDaIncricaoInconsistentes(IMediator mediator, IContextoAplicacao contextoAplicacao) : CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterRegistrosDaIncricaoInconsistentes
    {
        public Task<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>> Executar(long arquivoId)
        {
            return mediator.Send(new ObterRegistrosDaIncricaoInconsistentesQuery(QuantidadeRegistrosIgnorados, NumeroRegistros, arquivoId));
        }
    }
}
