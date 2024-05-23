using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao.ServicosFakes
{
    public class ObterUsuarioLogadoQueryHandlerFaker : IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>
    {
        public Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AoObterTotalNotificacaoNaoLidoMock.UsuarioLogado);
        }
    }
}
