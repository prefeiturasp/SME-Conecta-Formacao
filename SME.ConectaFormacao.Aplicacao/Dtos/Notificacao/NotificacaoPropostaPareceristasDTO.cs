using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

[ExcludeFromCodeCoverage]
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