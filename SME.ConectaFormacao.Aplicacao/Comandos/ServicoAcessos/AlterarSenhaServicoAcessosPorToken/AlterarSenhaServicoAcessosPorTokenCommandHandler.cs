using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSenhaServicoAcessosPorTokenCommandHandler : IRequestHandler<AlterarSenhaServicoAcessosPorTokenCommand, string>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public AlterarSenhaServicoAcessosPorTokenCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<string> Handle(AlterarSenhaServicoAcessosPorTokenCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.AlterarSenhaComTokenRecuperacao(request.Token, request.NovaSenha);
        }
    }
}
