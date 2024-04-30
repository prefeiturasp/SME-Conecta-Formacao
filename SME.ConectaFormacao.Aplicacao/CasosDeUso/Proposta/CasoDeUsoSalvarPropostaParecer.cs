using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaParecer : CasoDeUsoAbstrato, ICasoDeUsoSalvarPropostaParecer
    {
        public CasoDeUsoSalvarPropostaParecer(IMediator mediator) : base(mediator)
        {}
        
        public async Task<RetornoDTO> Executar(PropostaParecerCadastroDTO propostaParecerCadastroDto)
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            
            return await mediator.Send(new SalvarPropostaParecerCommand(propostaParecerCadastroDto, usuarioLogado.Id));
        }
    }
}