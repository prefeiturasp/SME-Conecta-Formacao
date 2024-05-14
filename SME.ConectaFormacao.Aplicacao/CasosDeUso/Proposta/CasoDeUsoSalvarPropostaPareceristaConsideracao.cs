using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaPareceristaConsideracao : CasoDeUsoAbstrato, ICasoDeUsoSalvarPropostaPareceristaConsideracao
    {
        public CasoDeUsoSalvarPropostaPareceristaConsideracao(IMediator mediator) : base(mediator)
        {}
        
        public async Task<RetornoDTO> Executar(PropostaPareceristaConsideracaoCadastroDTO propostaPareceristaConsideracaoCadastroDto)
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            
            return await mediator.Send(new SalvarPropostaPareceristaConsideracaoCommand(propostaPareceristaConsideracaoCadastroDto, usuarioLogado.Login));
        }
    }
}