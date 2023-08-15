using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommandHandler : IRequestHandler<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand, string>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<string> Handle(SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.SolicitarRecuperacaoSenha(request.Login);
        }
    }
}
