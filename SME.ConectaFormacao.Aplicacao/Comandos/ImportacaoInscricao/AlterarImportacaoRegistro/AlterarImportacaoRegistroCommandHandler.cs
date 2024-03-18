using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarImportacaoRegistroCommandHandler : IRequestHandler<AlterarImportacaoRegistroCommand, bool>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacaoArquivoRegistro;

        public AlterarImportacaoRegistroCommandHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacaoArquivoRegistro)
        {
            _repositorioImportacaoArquivoRegistro = repositorioImportacaoArquivoRegistro ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivoRegistro));
        }

        public async Task<bool> Handle(AlterarImportacaoRegistroCommand request, CancellationToken cancellationToken)
        {
            var importacaoArquivoRegistro = await _repositorioImportacaoArquivoRegistro.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            importacaoArquivoRegistro.Situacao = request.Situacao;
            importacaoArquivoRegistro.Conteudo = request.Conteudo;

            await _repositorioImportacaoArquivoRegistro.Atualizar(importacaoArquivoRegistro);

            return true;
        }
    }
}
