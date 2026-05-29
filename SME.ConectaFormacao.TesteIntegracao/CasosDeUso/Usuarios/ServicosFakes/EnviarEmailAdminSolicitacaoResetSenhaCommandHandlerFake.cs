using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.EnviarEmailAdminSolicitacaoResetSenha;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuarios.ServicosFakes
{
    public class EnviarEmailAdminSolicitacaoResetSenhaCommandHandlerFake : IRequestHandler<EnviarEmailAdminSolicitacaoResetSenhaCommand, bool>
    {
        public Task<bool> Handle(EnviarEmailAdminSolicitacaoResetSenhaCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}