using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao
{
    public class UsuarioExisteNoCoreSsoQueryFake : IRequestHandler<UsuarioExisteNoCoreSsoQuery, bool>
    {
        public async Task<bool> Handle(UsuarioExisteNoCoreSsoQuery request, CancellationToken cancellationToken)
        {
            return false;
        }
    }
}
