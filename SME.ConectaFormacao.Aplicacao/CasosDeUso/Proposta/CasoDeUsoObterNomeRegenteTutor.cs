using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterNomeRegenteTutor : CasoDeUsoAbstrato, ICasoDeUsoObterNomeRegenteTutor
    {
        public CasoDeUsoObterNomeRegenteTutor(IMediator mediator) : base(mediator)
        {
        }

        public async Task<string> Executar(string registroFuncional)
        {
            var retornoUsuario = await mediator.Send(new ObterNomeCpfProfissionalPorRegistroFuncionalQuery(registroFuncional));
            return retornoUsuario.Nome;
        }
    }
}