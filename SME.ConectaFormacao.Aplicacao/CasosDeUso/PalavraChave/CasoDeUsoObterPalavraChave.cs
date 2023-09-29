using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.PalavraChave
{
    public class CasoDeUsoObterPalavraChave : CasoDeUsoAbstrato, ICasoDeUsoObterPalavraChave
    {
        public CasoDeUsoObterPalavraChave(IMediator mediator) : base(mediator)
        {}
        public async Task<IEnumerable<PalavraChaveDTO>> Executar()
        {
            return await mediator.Send(ObterPalavraChaveQuery.Instance);
        }
    }
}
