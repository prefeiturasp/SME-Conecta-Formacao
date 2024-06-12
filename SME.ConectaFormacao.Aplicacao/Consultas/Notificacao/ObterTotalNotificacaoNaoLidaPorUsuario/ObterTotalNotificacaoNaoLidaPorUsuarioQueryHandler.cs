using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTotalNotificacaoNaoLidaPorUsuarioQueryHandler : IRequestHandler<ObterTotalNotificacaoNaoLidaPorUsuarioQuery, long>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;

        public ObterTotalNotificacaoNaoLidaPorUsuarioQueryHandler(IRepositorioNotificacao repositorioNotificacao)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
        }

        public async Task<long> Handle(ObterTotalNotificacaoNaoLidaPorUsuarioQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioNotificacao.ObterTotalNaoLidoPorUsuario(request.Login);
        }
    }
}
