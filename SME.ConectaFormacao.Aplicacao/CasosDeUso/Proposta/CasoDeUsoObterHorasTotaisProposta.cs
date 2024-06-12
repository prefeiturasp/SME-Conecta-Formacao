using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterHorasTotaisProposta : CasoDeUsoAbstrato, ICasoDeUsoObterHorasTotaisProposta
    {
        public CasoDeUsoObterHorasTotaisProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            var lista = Enum.GetValues(typeof(HorasTotaisProposta))
                .Cast<HorasTotaisProposta>()
                .Select(t => new RetornoListagemDTO
                {
                    Id = (short)t,
                    Descricao = t.Nome()
                });

            return await Task.FromResult(lista);
        }
    }
}