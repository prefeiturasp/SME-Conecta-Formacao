using MediatR;
using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoImportacaoInscricaoCursista : CasoDeUsoAbstrato, ICasoDeUsoImportacaoArquivoInscricaoCursista
    {
        public CasoDeUsoImportacaoInscricaoCursista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(IFormFile arquivo, long propostaId)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new NegocioException(MensagemNegocio.ARQUIVO_VAZIO);

            if (arquivo.ContentType.NaoEhArquivoXlsx())
                throw new NegocioException(MensagemNegocio.SOMENTE_ARQUIVO_XLSX_SUPORTADO);

            var importacaoArquivoDTO = new ImportacaoArquivoDTO(propostaId, arquivo.FileName, TipoImportacaoArquivo.Inscricao_Manual, SituacaoImportacaoArquivo.CarregamentoInicial);

            importacaoArquivoDTO.Id = await mediator.Send(new InserirImportacaoArquivoCommand(importacaoArquivoDTO));

            await mediator.Send(new InserirConteudoArquivoInscricaoCursistaCommand(importacaoArquivoDTO.Id, arquivo.OpenReadStream()));

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarImportacaoInscricaoCursistaValidar, importacaoArquivoDTO));

            return RetornoDTO.RetornarSucesso(MensagemNegocio.ARQUIVO_IMPORTADO_COM_SUCESSO, importacaoArquivoDTO.Id);
        }
    }
}
