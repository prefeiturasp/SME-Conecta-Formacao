using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Eventos.Codaf
{
    public record CodafListaPresencaDevolvidaEvento(
        CodafListaPresenca CodafListaPresenca,
        CodafComentarioListaPresenca Comentario) : INotification;
}
