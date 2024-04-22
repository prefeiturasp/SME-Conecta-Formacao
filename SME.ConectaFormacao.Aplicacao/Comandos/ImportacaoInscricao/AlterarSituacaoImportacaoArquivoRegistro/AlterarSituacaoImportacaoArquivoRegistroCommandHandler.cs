using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoImportacaoArquivoRegistroCommandHandler : IRequestHandler<AlterarSituacaoImportacaoArquivoRegistroCommand, bool>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacaoArquivoRegistro;

        public AlterarSituacaoImportacaoArquivoRegistroCommandHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacaoArquivoRegistro)
        {
            _repositorioImportacaoArquivoRegistro = repositorioImportacaoArquivoRegistro ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivoRegistro));
        }

        public async Task<bool> Handle(AlterarSituacaoImportacaoArquivoRegistroCommand request, CancellationToken cancellationToken)
        {
            var importacaoArquivoRegistro = await _repositorioImportacaoArquivoRegistro.ObterPorId(request.Id);

            if (importacaoArquivoRegistro.EhNulo())
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            importacaoArquivoRegistro.DefinirSituacaoErro(request.Situacao, request.Erro);

            await _repositorioImportacaoArquivoRegistro.Atualizar(importacaoArquivoRegistro);

            return true;
        }
    }
}
