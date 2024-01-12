using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomatica : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomatica
    {
        public CasoDeUsoRealizarInscricaoAutomatica(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var formacoesResumidas = await mediator.Send(new ObterPropostaPorTipoInscricaoESituacaoQuery(new []{TipoInscricao.Automatica,TipoInscricao.AutomaticaJEIF}, SituacaoProposta.Publicada));
            
            //Coletar do EOL os rfs alvos com base no cargo, função, ano, componente e modalidade
            
            //Realizar as inscrições nas turmas

            return false;
        }
    }
}
