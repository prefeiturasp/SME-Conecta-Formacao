using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterUsuarioLogadoQueryHandlerFakerEnviarParecerista : IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>
    {
        public Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaEnviarPareceristaMock.UsuarioLogado);
        }
    }
}
