using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoObterTiposEmail : CasoDeUsoAbstrato, ICasoDeUsoObterTiposEmail
    {
        public CasoDeUsoObterTiposEmail(IMediator mediator) : base(mediator)
        {
        }

        public Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            var lista = Enum.GetValues(typeof(TipoEmail))
                            .Cast<TipoEmail>()
                            .Select(t => new RetornoListagemDTO
                            {
                                Id = (short)t,
                                Descricao = t.Nome()
                            });

            return Task.FromResult(lista);
        }
    }
}
