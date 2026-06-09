using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Interfaces.FuncionarioExterno.ObterFuncionarioExternoPorCpf;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.FuncionarioExterno.ObterFuncionarioExternoPorCpf
{
    public class CasoDeUsoObterFuncionarioExternoPorCpf : CasoDeUsoAbstrato, ICasoDeUsoObterFuncionarioExternoPorCpf
    {

        public CasoDeUsoObterFuncionarioExternoPorCpf(IMediator mediator) : base(mediator)
        {
        }

        public async Task<FuncionarioExternoDTO?> Executar(string cpf)
        {
            var contratos = await mediator.Send(new ObterDadosFuncionarioExternoQuery(cpf));

            if (contratos?.Any() ?? false)
            {
                var primeiroContrato = contratos.FirstOrDefault();
                var ues = contratos.Select(x => new RetornoListagemDTO() { Id = Convert.ToInt64(x.CodigoUE), Descricao = x.NomeUe }).DistinctBy(x => x.Id).ToList();

                return new FuncionarioExternoDTO(
                    primeiroContrato!.NomePessoa,
                    primeiroContrato.Cpf,
                    primeiroContrato.CodigoUE,
                    primeiroContrato.NomeUe,
                    ues);
            }

            return null;
        }
    }
}