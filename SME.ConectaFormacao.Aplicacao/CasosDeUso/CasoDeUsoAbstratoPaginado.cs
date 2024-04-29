using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso
{
    public class CasoDeUsoAbstratoPaginado : CasoDeUsoAbstrato
    {
        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
        public int QuantidadeRegistrosIgnorados { get; set; }

        public CasoDeUsoAbstratoPaginado(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator)
        {
            NumeroPagina = int.TryParse(contextoAplicacao.ObterVariavel<string>("NumeroPagina"), out int numeroPagina) ? numeroPagina : 1;
            NumeroRegistros = int.TryParse(contextoAplicacao.ObterVariavel<string>("NumeroRegistros"), out int numeroRegistros) ? numeroRegistros : 10;
            CarregarRegistroIgnorados();
        }

        private void CarregarRegistroIgnorados()
        {
            NumeroPagina = NumeroPagina < 1 ? 1 : NumeroPagina;
            NumeroRegistros = NumeroRegistros < 1 ? 0 : NumeroRegistros;
            QuantidadeRegistrosIgnorados = (NumeroPagina - 1) * NumeroRegistros;
        }
    }
}
