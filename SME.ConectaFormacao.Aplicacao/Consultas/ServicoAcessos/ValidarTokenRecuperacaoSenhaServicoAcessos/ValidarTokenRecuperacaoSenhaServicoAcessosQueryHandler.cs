using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryHandler : IRequestHandler<ValidarTokenRecuperacaoSenhaServicoAcessosQuery, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public ValidarTokenRecuperacaoSenhaServicoAcessosQueryHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(ValidarTokenRecuperacaoSenhaServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.TokenRecuperacaoSenhaEstaValido(request.Token);
        }
    }
}
