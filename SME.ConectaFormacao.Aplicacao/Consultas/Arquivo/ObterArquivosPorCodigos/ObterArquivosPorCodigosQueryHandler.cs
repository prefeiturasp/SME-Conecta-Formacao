using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterArquivosPorCodigosQueryHandler : IRequestHandler<ObterArquivosPorCodigosQuery, IEnumerable<Arquivo>>
    {
        private readonly IRepositorioArquivo _repositorioArquivo;

        public ObterArquivosPorCodigosQueryHandler(IRepositorioArquivo repositorioArquivo)
        {
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
        }

        public Task<IEnumerable<Arquivo>> Handle(ObterArquivosPorCodigosQuery request, CancellationToken cancellationToken)
        {
            return _repositorioArquivo.ObterPorCodigos(request.Codigos);
        }
    }
}
