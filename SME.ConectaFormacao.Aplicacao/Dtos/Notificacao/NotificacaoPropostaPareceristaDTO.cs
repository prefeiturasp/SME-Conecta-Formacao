using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

[ExcludeFromCodeCoverage]
public class NotificacaoPropostaPareceristaDTO
{
    public long PropostaId { get; set; }
    public PropostaPareceristaResumidoDTO Parecerista { get; set; }

    public NotificacaoPropostaPareceristaDTO(long propostaId, PropostaPareceristaResumidoDTO parecerista)
    {
        PropostaId = propostaId;
        Parecerista = parecerista;
    }
}