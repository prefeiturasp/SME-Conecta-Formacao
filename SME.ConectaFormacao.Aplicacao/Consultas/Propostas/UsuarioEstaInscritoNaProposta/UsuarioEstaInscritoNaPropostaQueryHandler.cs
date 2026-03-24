using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class UsuarioEstaInscritoNaPropostaQueryHandler : IRequestHandler<UsuarioEstaInscritoNaPropostaQuery, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public UsuarioEstaInscritoNaPropostaQueryHandler(IRepositorioInscricao repositorioInscricao)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<bool> Handle(UsuarioEstaInscritoNaPropostaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioInscricao.UsuarioEstaInscritoNaProposta(request.UsuarioId, request.PropostaId);
        }
    }
}
