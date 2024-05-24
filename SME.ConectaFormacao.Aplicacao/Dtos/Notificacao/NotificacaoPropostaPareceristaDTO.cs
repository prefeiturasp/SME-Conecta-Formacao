using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

public class NotificacaoPropostaPareceristaDTO
{
    public long PropostaId { get; set; }
    public PropostaPareceristaResumidoDTO Parecerista { get; set; }

    public NotificacaoPropostaPareceristaDTO(long propostaId,PropostaPareceristaResumidoDTO parecerista)
    {
        PropostaId = propostaId;
        Parecerista = parecerista;
    }
}