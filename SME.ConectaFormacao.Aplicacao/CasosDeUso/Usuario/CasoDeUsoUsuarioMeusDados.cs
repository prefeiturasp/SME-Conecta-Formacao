using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioMeusDados : CasoDeUsoAbstrato, ICasoDeUsoUsuarioMeusDados
    {
        public CasoDeUsoUsuarioMeusDados(IMediator mediator) : base(mediator)
        {
        }

        public async Task<DadosUsuarioDTO> Executar(string login)
        {
            return await mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login));
        }
    }
}
