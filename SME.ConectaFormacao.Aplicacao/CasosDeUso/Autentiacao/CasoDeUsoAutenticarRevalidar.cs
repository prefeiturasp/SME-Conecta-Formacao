using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao
{
    public class CasoDeUsoAutenticarRevalidar : CasoDeUsoAbstrato, ICasoDeUsoAutenticarRevalidar
    {
        public CasoDeUsoAutenticarRevalidar(IMediator mediator) : base(mediator)
        {
        }

        public async Task<UsuarioPerfisRetornoDTO> Executar(string token)
        {
            return await mediator.Send(new RevalidarTokenServicoAcessosQuery(token));
        }
    }
}
