using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo
{
    public class AlterarSituacaoImportacaoArquivoCommandHandler(IRepositorioImportacaoArquivo repositorioImportacaoArquivo) : 
        IRequestHandler<AlterarSituacaoImportacaoArquivoCommand, bool>
    {
        public async Task<bool> Handle(AlterarSituacaoImportacaoArquivoCommand request, CancellationToken cancellationToken)
        {
            var importacaoArquivo = await repositorioImportacaoArquivo.ObterPorId(request.Id) ??
                                    throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_NAO_LOCALIZADA);
            importacaoArquivo.DefinirSituacao(request.Situacao);

            await repositorioImportacaoArquivo.Atualizar(importacaoArquivo);

            return true;
        }
    }
}
