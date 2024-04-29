using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterArquivosInscricaoImportados : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterArquivosInscricaoImportados
    {
        public CasoDeUsoObterArquivosInscricaoImportados(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public Task<PaginacaoResultadoDTO<ArquivoInscricaoImportadoDTO>> Executar(long propostaId)
        {
            return mediator.Send(new ObterArquivosInscricaoImportadosQuery(QuantidadeRegistrosIgnorados, NumeroRegistros, propostaId));
        }
    }
}
