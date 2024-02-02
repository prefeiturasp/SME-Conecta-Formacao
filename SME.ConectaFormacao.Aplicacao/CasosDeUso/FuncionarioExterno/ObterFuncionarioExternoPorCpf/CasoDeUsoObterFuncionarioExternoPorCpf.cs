using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno;
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

        public async Task<FuncionarioExternoDTO> Executar(string cpf)
        {
            var contratos  = await mediator.Send(new ObterDadosFuncionarioExternoQuery(cpf));

            var ues = contratos.Select(x => new RetornoListagemDTO() {Id = Convert.ToInt64(x.CodigoUE), Descricao = x.NomeUe}).DistinctBy(x =>x.Id);

            return new FuncionarioExternoDTO(
                contratos.FirstOrDefault().NomePessoa,
                contratos.FirstOrDefault().Cpf,
                contratos.FirstOrDefault().CodigoUE,
                contratos.FirstOrDefault().NomeUe,
                ues);

        }
    }
}