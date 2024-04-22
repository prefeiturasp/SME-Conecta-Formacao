using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario
{
    public class CasoDeUsoObterUsuariosAdminDf : CasoDeUsoAbstrato, ICasoDeUsoObterUsuariosAdminDf
    {
        public CasoDeUsoObterUsuariosAdminDf(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<UsuarioAdminDfDTO>> Executar()
        {
            return await mediator.Send(new ObterUsuariosAdminDfQuery());
        }
    }
}