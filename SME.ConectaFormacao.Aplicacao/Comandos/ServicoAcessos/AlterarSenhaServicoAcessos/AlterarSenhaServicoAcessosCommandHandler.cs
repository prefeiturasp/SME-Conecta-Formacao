using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSenhaServicoAcessosCommandHandler : IRequestHandler<AlterarSenhaServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public AlterarSenhaServicoAcessosCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(AlterarSenhaServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.AlterarSenha(request.Login, request.SenhaAtual, request.NovaSenha);
        }
    }
}
