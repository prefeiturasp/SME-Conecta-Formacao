using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.FuncionarioExterno.ObterFuncionarioExternoPorCpf
{
    public class CasoDeUsoObterFuncionarioExternoPorCpf : CasoDeUsoAbstrato, ICasoDeUsoObterFuncionarioExternoPorCpf
    {
        
        public CasoDeUsoObterFuncionarioExternoPorCpf(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<FuncionarioExternoServicoEol>> Executar(string cpf)
        {
            return await mediator.Send(new ObterDadosFuncionarioExternoQuery(cpf));
        }
    }
}