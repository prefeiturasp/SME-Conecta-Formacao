using MediatR;
using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo
{
    public class CasoDeUsoImportacaoArquivoInscricaoCursista : CasoDeUsoAbstrato, ICasoDeUsoImportacaoArquivoInscricaoCursista
    {
        public CasoDeUsoImportacaoArquivoInscricaoCursista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(IFormFile arquivo, long propostaId)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new NegocioException(MensagemNegocio.ARQUIVO_VAZIO);
            
            if (arquivo.ContentType.NaoEhArquivoXlsx())
                throw new NegocioException(MensagemNegocio.SOMENTE_ARQUIVO_XLSX_SUPORTADO);

            var importacaoArquivoDTO = new ImportacaoArquivoDTO(propostaId, arquivo.FileName, TipoImportacaoArquivo.Inscricao_Manual, SituacaoImportacaoArquivo.Enviado);

            var id = await mediator.Send(new InserirImportacaoArquivoCommand(importacaoArquivoDTO));
            
            var caminho = await mediator.Send(new InserirConteudoArquivoInscricaoCursistaCommand(id, arquivo.OpenReadStream()));

            return RetornoDTO.RetornarSucesso(MensagemNegocio.ARQUIVO_IMPORTADO_COM_SUCESSO, id);
        }
    }
}
