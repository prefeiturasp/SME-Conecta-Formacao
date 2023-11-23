using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    public class ObterUsuarioLogadoQueryFake : IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>
    {
        public Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AutenticacaoMock.UsuarioLogado);
        }
    }
}
