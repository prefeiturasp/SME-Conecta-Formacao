using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoArquivosParaCanceladoCommandHandler : IRequestHandler<AlterarSituacaoArquivosParaCanceladoCommand, bool>
    {
        private readonly IRepositorioImportacaoArquivo _repositorioImportacaoArquivo;

        public AlterarSituacaoArquivosParaCanceladoCommandHandler(IRepositorioImportacaoArquivo repositorioImportacaoArquivo)
        {
            _repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
        }

        public async Task<bool> Handle(AlterarSituacaoArquivosParaCanceladoCommand request, CancellationToken cancellationToken)
        {
            var arquivo = await _repositorioImportacaoArquivo.ObterPorId(request.ArquivoImportacaoId);

            if (arquivo.EhNulo())
                throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO);

            arquivo.DefinirSituacao(SituacaoImportacaoArquivo.Cancelado);

            await _repositorioImportacaoArquivo.Atualizar(arquivo);

            return true;
        }
    }
}
