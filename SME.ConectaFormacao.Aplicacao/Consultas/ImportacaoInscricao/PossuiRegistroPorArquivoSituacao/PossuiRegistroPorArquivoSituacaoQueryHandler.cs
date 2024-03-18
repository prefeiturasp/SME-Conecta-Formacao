using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PossuiRegistroPorArquivoSituacaoQueryHandler : IRequestHandler<PossuiRegistroPorArquivoSituacaoQuery, bool>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacaoRegistro;

        public PossuiRegistroPorArquivoSituacaoQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacaoRegistro)
        {
            _repositorioImportacaoRegistro = repositorioImportacaoRegistro ?? throw new ArgumentNullException(nameof(repositorioImportacaoRegistro));
        }

        public Task<bool> Handle(PossuiRegistroPorArquivoSituacaoQuery request, CancellationToken cancellationToken)
        {
            return _repositorioImportacaoRegistro.TodosRegistroForamProcessadosDoArquivo(request.ImportacaoArquivoId, request.SituacaoVerificar);
        }
    }
}
