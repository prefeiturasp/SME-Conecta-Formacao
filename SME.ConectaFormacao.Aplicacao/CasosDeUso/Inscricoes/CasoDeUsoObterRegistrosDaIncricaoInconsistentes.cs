using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterRegistrosDaIncricaoInconsistentes : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterRegistrosDaIncricaoInconsistentes
    {
        public CasoDeUsoObterRegistrosDaIncricaoInconsistentes(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public Task<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDTO>> Executar(long arquivoId)
        {
            return mediator.Send(new ObterRegistrosDaIncricaoInconsistentesQuery(QuantidadeRegistrosIgnorados, NumeroRegistros, arquivoId));
        }
    }
}
