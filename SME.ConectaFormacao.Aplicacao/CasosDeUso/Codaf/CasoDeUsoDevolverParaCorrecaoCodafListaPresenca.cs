using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoDevolverParaCorrecaoCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioCodafComentarioListaPresenca repositorioComentarioCodafListaPresenca,
        ITransacao transacao,
        IGerenciadorMovimentacaoCodafService gerenciadorMovimentacaoCodafService,
        IRepositorioCodafMovimentacaoListaPresenca repositorioCodafMovimentacao,
        IRepositorioUsuario repositorioUsuario,
        IMediator mediator,
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,
        IMapper mapper) :
        ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafListaPresencaId, string justificativa)
        {
            if (codafListaPresencaId <= 0)
                return Erro.Validacao("O Id da lista de presença Codaf deve ser informado.");
            if (string.IsNullOrWhiteSpace(justificativa))
                return Erro.Validacao("A justificativa para devolução da lista de presença Codaf deve ser informada.");

            var codafListaPresenca = await repositorioCodafListaPresenca.ObterPorId(codafListaPresencaId);
            if (codafListaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença Codaf não encontrada para o Id informado.");

            if (!codafListaPresenca.PodeSerDevolvidaParaCorrecao())
                return Erro.Validacao("A lista de presença Codaf deve estar com status 'Enviada para DF' para ser devolvida para correção.");

            using var transacaoDb = transacao.Iniciar();
            try
            {
                codafListaPresenca.MarcarComoDevolvidaParaCorrecao();
                await repositorioCodafListaPresenca.Atualizar(codafListaPresenca);
                var correlacaoId = Guid.NewGuid();
                var comentario = new CodafComentarioListaPresenca
                {
                    Comentario = justificativa,
                    CodafListaPresencaId = codafListaPresenca.Id,
                    NotificacaoCorrelacaoId = correlacaoId
                };
                var idComentario = await repositorioComentarioCodafListaPresenca.Inserir(comentario);
                await gerenciadorMovimentacaoCodafService.RegistrarMovimentacaoAsync(codafListaPresenca, idComentario);
                await EnviarNotificacoesAsync(transacaoDb, codafListaPresenca, comentario);
                transacaoDb.Commit();
                return true;
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Ocorreu um erro ao devolver a lista de presença Codaf para a área promotora.");
            }
        }

        private async Task EnviarNotificacoesAsync(IDbTransaction transacao, CodafListaPresenca codafListaPresenca, CodafComentarioListaPresenca comentarioListaPresenca)
        {
            var ultimaMovimentacao = await repositorioCodafMovimentacao.ObterUltimaMovimentacaoPorListaPresencaStatusAsync(
                codafListaPresenca.Id, StatusCodafListaPresenca.AguardandoDf);

            var dadosUsuarioParaNotificar = await repositorioUsuario.ObterPorLogin(ultimaMovimentacao!.CriadoLogin);
            var dadosUsuarioAtual = await mediator.Send(new ObterUsuarioLogadoQuery());

            var notificacaoUsuario = new NotificacaoUsuario(dadosUsuarioParaNotificar!.Login, dadosUsuarioParaNotificar!.Nome, dadosUsuarioParaNotificar!.Email);

            var notificacao = new Notificacao
            {
                Usuarios = [notificacaoUsuario],
                CorrelacaoId = comentarioListaPresenca.NotificacaoCorrelacaoId,
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Codaf,
                TipoEnvio = NotificacaoTipoEnvio.SignalR,
                TipoOrigem = NotificacaoTipoOrigem.DevolucaoParaCorrecaoCodaf,
                Titulo = $"O registro codaf código {codafListaPresenca.Id} foi retornado pela DF",
                Mensagem =
                $"""
                O admin {dadosUsuarioAtual!.Nome} {dadosUsuarioAtual!.Login} atuou na Devolução do CODAF à área promotora. 
                Clique <a href="/formacoes/lista-presenca-codaf/editar/{codafListaPresenca.Id}">aqui</a> para acessar o CODAF.
                <br />
                <strong>Comentário:</strong> 
                <br />
                {comentarioListaPresenca.Comentario}
                """,
                Parametros = new { codafListaPresencaId = codafListaPresenca.Id, comentarioId = comentarioListaPresenca.Id, correlacaoId = comentarioListaPresenca.NotificacaoCorrelacaoId }.ObjetoParaJson()
            };
            await repositorioNotificacao.Inserir(notificacao);
            await repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacao.Id);

            foreach (var usuario in notificacao.Usuarios)
            {
                if (!string.IsNullOrWhiteSpace(usuario.Email))
                {
                    var destinatario = mapper.Map<EnviarEmailDto>(usuario);
                    destinatario.Titulo = notificacao.Titulo;
                    destinatario.Texto = notificacao.Mensagem;
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario));
                }
            }
        }
    }
}
