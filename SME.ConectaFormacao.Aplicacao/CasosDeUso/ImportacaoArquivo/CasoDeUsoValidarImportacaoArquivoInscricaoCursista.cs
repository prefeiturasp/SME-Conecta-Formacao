using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
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

            var qtdeRegistros = await ObterParametroQtdeRegistrosAProcessar();

            var importacaoItens = await ObterRegistrosParaValidar(qtdeRegistros, importacaoArquivoId);

            foreach (var item in importacaoItens.Items)
                await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista, inscricaoAutomaticaDTO));

            return true;
        }

        private async Task<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>> ObterRegistrosParaValidar(int qtdeRegistros, long importacaoArquivoId)
        {
            return await mediator.Send(new ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery(0, qtdeRegistros, importacaoArquivoId));
        }

        private async Task<int> ObterParametroQtdeRegistrosAProcessar()
        {
            var parametro = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista, DateTimeExtension.HorarioBrasilia().Year));
            return int.Parse(parametro.Valor);
        }
    }
}
