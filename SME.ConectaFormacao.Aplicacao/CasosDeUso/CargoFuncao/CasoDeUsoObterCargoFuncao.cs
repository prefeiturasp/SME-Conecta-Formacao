using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterCargoFuncao : CasoDeUsoAbstrato, ICasoDeUsoObterCargoFuncao
    {
        public CasoDeUsoObterCargoFuncao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<CargoFuncaoDTO>> Executar(CargoFuncaoTipo? tipo, bool exibirOpcaoOutros)
        {
            var cargosFuncoes = await mediator.Send(new ObterCargoFuncaoQuery(tipo));

            if (exibirOpcaoOutros)
            {
                var cargosFuncoesLista = cargosFuncoes.ToList();
                cargosFuncoesLista.Add(new CargoFuncaoDTO
                {
                    Id = (long)OpcaoListagem.Outros,
                    Nome = OpcaoListagem.Outros.Nome()
                });

                cargosFuncoes = cargosFuncoesLista;
            }

            return cargosFuncoes;
        }
    }
}
