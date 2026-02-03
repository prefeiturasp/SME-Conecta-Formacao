using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto
{
    public readonly record struct NotificacaoPersistidaEvento(
        Guid CorrelacaoId,
        NotificacaoTipoOrigem TipoOrigem,
        DateTime DataConfirmacao,
        long NotificacaoId);
}
