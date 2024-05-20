using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario
{
    public class CasoDeUsoObterUsuariosAdminDf : CasoDeUsoAbstrato, ICasoDeUsoObterUsuariosAdminDf
    {
        public CasoDeUsoObterUsuariosAdminDf(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Executar()
        {
            return await mediator.Send(new ObterUsuariosAdminDfQuery());
        }
    }
}