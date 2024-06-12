using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterUsuarioLogadoQueryHandlerFakerEnviarParecerista : IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>
    {
        public Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaEnviarPareceristaMock.UsuarioLogado);
        }
    }
}
