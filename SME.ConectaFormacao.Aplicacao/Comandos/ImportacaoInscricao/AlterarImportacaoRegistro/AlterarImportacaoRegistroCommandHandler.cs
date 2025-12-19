using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro
{
    public class AlterarImportacaoRegistroCommandHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacaoArquivoRegistro) : 
        IRequestHandler<AlterarImportacaoRegistroCommand, bool>
    {
        public async Task<bool> Handle(AlterarImportacaoRegistroCommand request, CancellationToken cancellationToken)
        {
            var importacaoArquivoRegistro = await repositorioImportacaoArquivoRegistro.ObterPorId(request.AlterarImportacaoRegistroDto.Id) ??
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            importacaoArquivoRegistro.Situacao = request.AlterarImportacaoRegistroDto.Situacao;
            importacaoArquivoRegistro.Conteudo = request.AlterarImportacaoRegistroDto.Conteudo;
            importacaoArquivoRegistro.Erro = request.AlterarImportacaoRegistroDto.Erro;

            await repositorioImportacaoArquivoRegistro.Atualizar(importacaoArquivoRegistro);

            return true;
        }
    }
}
