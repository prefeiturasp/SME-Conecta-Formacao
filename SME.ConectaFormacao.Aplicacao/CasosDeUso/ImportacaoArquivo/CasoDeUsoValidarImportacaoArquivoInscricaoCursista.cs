using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo
{
    public class CasoDeUsoValidarImportacaoArquivoInscricaoCursista : CasoDeUsoAbstrato, ICasoDeUsoValidarImportacaoArquivoInscricaoCursista
    {
        
        public CasoDeUsoValidarImportacaoArquivoInscricaoCursista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoId = Convert.ToInt64(param.Mensagem);

            var qtdeRegistros = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista, DateTimeExtension.HorarioBrasilia().Year));

            var importacaoItens = await mediator.Send(new ObterArquivosInscricaoImportadosQuery(QuantidadeRegistrosIgnorados, NumeroRegistros, propostaId));

            return true;
        }
    }
}
