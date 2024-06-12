using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Comandos.Usuario.AlterarEmailEducacional;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoUsuarioAlterarEmailEducacional : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarEmailEducacional
    {
        public CasoDeUsoUsuarioAlterarEmailEducacional(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, string email)
        {
            return await mediator.Send(new AlterarEmailEducacionalCommand(email, login));
        }
    }
}