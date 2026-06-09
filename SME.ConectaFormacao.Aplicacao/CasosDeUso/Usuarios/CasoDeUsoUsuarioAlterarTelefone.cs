using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarTelefone : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarTelefone
    {
        public CasoDeUsoUsuarioAlterarTelefone(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, string telefone)
        {
            var telefoneFoiAlterado = await mediator.Send(new SalvarUsuarioTelefoneParcialCommand(login, telefone));
            return telefoneFoiAlterado;
        }
    }
}
