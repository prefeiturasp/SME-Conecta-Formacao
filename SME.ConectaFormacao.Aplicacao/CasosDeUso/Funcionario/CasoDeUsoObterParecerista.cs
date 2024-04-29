using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario
{
    public class CasoDeUsoObterParecerista : CasoDeUsoAbstrato, ICasoDeUsoObterParecerista
    {
        public CasoDeUsoObterParecerista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Executar()
        {
            return await mediator.Send(new ObterUsuariosPareceristasQuery());
        }
    }
}