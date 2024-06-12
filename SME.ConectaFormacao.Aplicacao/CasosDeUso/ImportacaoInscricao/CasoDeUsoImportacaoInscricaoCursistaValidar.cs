using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoImportacaoInscricaoCursistaValidar : CasoDeUsoAbstrato, ICasoDeUsoImportacaoInscricaoCursistaValidar
    {

        public CasoDeUsoImportacaoInscricaoCursistaValidar(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoDto = param.ObterObjetoMensagem<ImportacaoArquivoDTO>()
                                            ?? throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_NAO_LOCALIZADA);

            var qtdeRegistros = await ObterParametroQtdeRegistrosAProcessar();

            var registrosPaginados = await ObterRegistrosParaValidar(qtdeRegistros, importacaoArquivoDto.Id);

            var registrosValidados = 0;

            while (registrosValidados < registrosPaginados.TotalRegistros)
            {
                foreach (var item in registrosPaginados.Items)
                {
                    item.PropostaId = importacaoArquivoDto.PropostaId;
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem, item));
                }

                registrosValidados = registrosPaginados.Items.Count();

                registrosPaginados = await ObterRegistrosParaValidar(qtdeRegistros, importacaoArquivoDto.Id, registrosValidados);
            }

            await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoDto.Id, SituacaoImportacaoArquivo.Validando));

            return true;
        }

        private async Task<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>> ObterRegistrosParaValidar(int qtdeRegistros, long importacaoArquivoId, int qtdeRegistroIgnorados = 0)
        {
            return await mediator.Send(new ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery(
                qtdeRegistroIgnorados,
                qtdeRegistros,
                importacaoArquivoId, null));
        }

        private async Task<int> ObterParametroQtdeRegistrosAProcessar()
        {
            var parametro = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(
                TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista,
                DateTimeExtension.HorarioBrasilia().Year));

            return int.Parse(parametro.Valor);
        }
    }
}
