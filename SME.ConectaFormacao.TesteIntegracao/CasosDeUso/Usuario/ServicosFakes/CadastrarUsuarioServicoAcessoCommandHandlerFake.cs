using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao
{
    public class CadastrarUsuarioServicoAcessoCommandHandlerFake : IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>
    {
        public async Task<bool> Handle(CadastrarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
