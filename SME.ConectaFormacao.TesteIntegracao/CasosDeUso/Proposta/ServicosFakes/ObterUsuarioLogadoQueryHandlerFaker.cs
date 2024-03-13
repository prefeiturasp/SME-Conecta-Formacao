using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterUsuarioLogadoQueryHandlerFaker : IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>
    {
        public Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Dominio.Entidades.Usuario()
            {
                Nome = PropostaInformacoesCadastranteMock.UsuarioLogadoNome,
                Email = PropostaInformacoesCadastranteMock.UsuarioLogadoEmail,
            });
        }
    }
}
