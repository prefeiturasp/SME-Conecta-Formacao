using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterCriterioValidacaoInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterCriterioValidacaoInscricao
    {
        public CasoDeUsoObterCriterioValidacaoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<CriterioValidacaoInscricaoDTO>> Executar(bool exibirOpcaoOutros)
        {
            var criterios = await mediator.Send(ObterCriterioValidacaoInscricaoQuery.Instancia);
            if (exibirOpcaoOutros)
            {
                var criteriosLista = criterios.ToList();
                criteriosLista.Add(new CriterioValidacaoInscricaoDTO
                {
                    Id = (long)OpcaoListagem.Outros,
                    Nome = OpcaoListagem.Outros.Nome()
                });

                criterios = criteriosLista;
            }

            return criterios;
        }
    }
}
