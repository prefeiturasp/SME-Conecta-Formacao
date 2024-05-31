using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

public class NotificacaoPropostaPareceristasDTO
{
    public long PropostaId { get; set; }
    public IEnumerable<PropostaPareceristaResumidoDTO> Pareceristas { get; set; }

    public NotificacaoPropostaPareceristasDTO(long propostaId, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
    {
        PropostaId = propostaId;
        Pareceristas = pareceristas;
    }
}