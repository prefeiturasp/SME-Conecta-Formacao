using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo
{
    public class CasoDeUsoArquivoExcluir : CasoDeUsoAbstrato, ICasoDeUsoArquivoExcluir
    {
        public CasoDeUsoArquivoExcluir(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(Guid[] codigos)
        {
            var arquivos = await mediator.Send(new ObterArquivosPorCodigosQuery(codigos));
            if (!arquivos.Any())
                throw new NegocioException(MensagemNegocio.ARQUIVO_NENHUM_ARQUIVO_ENCONTRADO, System.Net.HttpStatusCode.NotFound);

            return await mediator.Send(new RemoverArquivosCommand(arquivos));
        }
    }
}
