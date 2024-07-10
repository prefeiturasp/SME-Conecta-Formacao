using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Usuario.AlterarEmailEducacional;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioAlterarTipoEmail : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarTipoEmail
    {
        public CasoDeUsoUsuarioAlterarTipoEmail(IMediator mediator) : base(mediator)
        {}

        public async Task<bool> Executar(string login, int tipo)
        {
            var alterasdoComSucesso = await mediator.Send(new AlterarTipoEmailCommand(tipo, login))
                                      && await mediator.Send(new AlterarEmailEduAoAlterarNomeTipoEmailCommand(login));
            return alterasdoComSucesso;
        }
            
    }
}
