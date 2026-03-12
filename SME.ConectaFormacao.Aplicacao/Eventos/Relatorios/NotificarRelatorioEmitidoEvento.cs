using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Eventos.Relatorios
{
    public record NotificarRelatorioEmitidoEvento(NotificacaoDTO Notificacao, List<Usuario> UsuariosAlvo) : INotification;
}