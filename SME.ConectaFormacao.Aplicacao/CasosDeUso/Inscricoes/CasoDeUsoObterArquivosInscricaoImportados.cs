using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterArquivosInscricaoImportados : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterArquivosInscricaoImportados
    {
        public CasoDeUsoObterArquivosInscricaoImportados(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public Task<PaginacaoResultadoDto<ArquivoInscricaoImportadoDTO>> Executar(long propostaId)
        {
            return mediator.Send(new ObterArquivosInscricaoImportadosQuery(QuantidadeRegistrosIgnorados, NumeroRegistros, propostaId));
        }
    }
}
