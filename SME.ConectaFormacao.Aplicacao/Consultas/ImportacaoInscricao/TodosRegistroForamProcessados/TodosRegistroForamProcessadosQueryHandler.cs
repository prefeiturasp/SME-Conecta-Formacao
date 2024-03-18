using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TodosRegistroForamProcessadosQueryHandler : IRequestHandler<TodosRegistroForamProcessadosQuery, bool>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacaoRegistro;

        public TodosRegistroForamProcessadosQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacaoRegistro)
        {
            _repositorioImportacaoRegistro = repositorioImportacaoRegistro ?? throw new ArgumentNullException(nameof(repositorioImportacaoRegistro));
        }

        public Task<bool> Handle(TodosRegistroForamProcessadosQuery request, CancellationToken cancellationToken)
        {
            return _repositorioImportacaoRegistro.TodosRegistroForamProcessadosDoArquivo(request.ImportacaoArquivoId);
        }
    }
}
