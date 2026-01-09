using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Eventos.Codaf
{
    public record CodafListaPresencaDevolvidaEvento(
        long CodafListaPresencaId,
        CodafComentarioListaPresenca Comentario) : INotification;
}
