using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricoesConfirmadasQueryHandler : IRequestHandler<ObterInscricoesConfirmadasQuery, IEnumerable<Inscricao>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public ObterInscricoesConfirmadasQueryHandler(IRepositorioInscricao repositorioInscricao)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<IEnumerable<Inscricao>> Handle(ObterInscricoesConfirmadasQuery request,
            CancellationToken cancellationToken)
        {
            return await _repositorioInscricao.ObterInscricoesConfirmadas();
        }
    }
}