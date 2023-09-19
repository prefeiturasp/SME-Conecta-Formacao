using MediatR;
using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo
{
    public class CasoDeUsoArquivoCarregarTemporario : CasoDeUsoAbstrato, ICasoDeUsoArquivoCarregarTemporario
    {
        private const int DezMb = 10 * 1024 * 1024;
        public CasoDeUsoArquivoCarregarTemporario(IMediator mediator) : base(mediator)
        {
        }

        public async Task<ArquivoArmazenadoDTO> Executar(IFormFile arquivo)
        {
            if (arquivo.Length == 0)
                throw new NegocioException(MensagemNegocio.ARQUIVO_VAZIO);

            if (arquivo.Length > DezMb)
                throw new NegocioException(MensagemNegocio.ARQUIVO_MAIOR_QUE_10_MB);

            var arquivoDTO = new ArquivoDTO(arquivo.FileName, Guid.NewGuid(), TipoArquivo.Temp, arquivo.ContentType, arquivo);

            var id = await mediator.Send(new InserirArquivoCommand(arquivoDTO));
            var caminho = await mediator.Send(new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDTO));

            return new ArquivoArmazenadoDTO(id, arquivoDTO.Codigo, caminho);
        }
    }
}
