using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes
{
    internal class CadastrarUsuarioServicoAcessoCommandHandlerFaker : IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>
    {
        public Task<bool> Handle(CadastrarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
