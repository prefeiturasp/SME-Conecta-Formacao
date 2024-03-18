using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoRegistroImportacaoArquivoCommandHandler : IRequestHandler<AlterarSituacaoRegistroImportacaoArquivoCommand, bool>
    {

        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacaoRegistro;

        public AlterarSituacaoRegistroImportacaoArquivoCommandHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacaoRegistro)
        {
            _repositorioImportacaoRegistro = repositorioImportacaoRegistro ?? throw new ArgumentNullException(nameof(repositorioImportacaoRegistro));
        }
        public async Task<bool> Handle(AlterarSituacaoRegistroImportacaoArquivoCommand request, CancellationToken cancellationToken)
        {
            var importacaoRegistro = await _repositorioImportacaoRegistro.ObterPorId(request.RegistroImportacaoId);

            if (importacaoRegistro.EhNulo())
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            importacaoRegistro.DefinirSituacao(request.Situacao);

            await _repositorioImportacaoRegistro.Atualizar(importacaoRegistro);

            return true;
        }
    }
}
