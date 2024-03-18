using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoProcessarArquivoDeImportacaoInscricao : CasoDeUsoAbstrato, ICasoDeUsoProcessarArquivoDeImportacaoInscricao
    {
        public CasoDeUsoProcessarArquivoDeImportacaoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoId = long.Parse(param.Mensagem.ToString());
            var qtdeRegistros = await ObterParametroQtdeRegistrosAProcessar();
            var registrosPaginados = await ObterRegistrosParaValidar(qtdeRegistros, importacaoArquivoId);
            var qtdeRegistrosValidados = 0;

            if (registrosPaginados.TotalRegistros == 0)
                return false;

            await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoId, SituacaoImportacaoArquivo.Processando));

            while (qtdeRegistrosValidados < registrosPaginados.TotalRegistros)
            {
                foreach (var registro in registrosPaginados.Items)
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.ProcessarRegistroDoArquivoDeImportacaoInscricao, registro));

                qtdeRegistrosValidados = registrosPaginados.Items.Count();

                registrosPaginados = await ObterRegistrosParaValidar(qtdeRegistros, importacaoArquivoId, qtdeRegistrosValidados);
            }

            return true;
        }

        private async Task<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>> ObterRegistrosParaValidar(int qtdeRegistros, long importacaoArquivoId, int qtdeRegistroIgnorados = 0)
        {
            return await mediator.Send(new ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery(qtdeRegistroIgnorados, qtdeRegistros, importacaoArquivoId, ignorarSituacao: SituacaoImportacaoArquivoRegistro.Erro));
        }

        private async Task<int> ObterParametroQtdeRegistrosAProcessar()
        {
            var parametro = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista, DateTimeExtension.HorarioBrasilia().Year));
            return int.Parse(parametro.Valor);
        }
    }
}
