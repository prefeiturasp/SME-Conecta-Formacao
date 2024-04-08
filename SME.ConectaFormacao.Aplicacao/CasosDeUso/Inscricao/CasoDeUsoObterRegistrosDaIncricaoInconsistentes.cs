using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
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
