using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Relatorios;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios
{
    public class CasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao(
        IContextoAplicacao contexto,
        IMediator mediator,
        IRepositorioUsuario repositorioUsuario,
        TimeProvider timeProvider,
        IServicoLogs servicoLogs) : ICasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao
    {
        public async Task<Resultado> ExecutarAsync(FiltroRelatorioInscritosPorFormacaoDto filtro)
        {
            var resultadoValidacao = FiltroRelatorioInscritosValidador.Validar(filtro);
            if (!resultadoValidacao.Sucesso)
                return resultadoValidacao;

            var dadosUsuarioLogado = await repositorioUsuario.ObterPorLogin(contexto.LoginUsuario);
            var solicitacaoMensagem = new SolicitacaoRelatorioInscritosPorFormacaoMensagem(
                RelatorioId: Guid.NewGuid(),
                Solicitante: new(dadosUsuarioLogado!.Id, contexto.NomeUsuario, contexto.LoginUsuario),
                DataSolicitacao: timeProvider.GetLocalNow().DateTime,
                Filtros: filtro);
            try
            {
                var mensagemPublicada = await mediator.Send(new PublicarNaFilaRabbitCommand(
                    RotasRabbit.GerarRelatorioInscritosExcel, solicitacaoMensagem, solicitacaoMensagem.RelatorioId, dadosUsuarioLogado, true));

                if (!mensagemPublicada)
                    return new Erro(TipoFalha.ErroInterno, "Erro ao solicitar relatório");
            }
            catch (Exception ex)
            {
                await servicoLogs.Enviar(ex, $"Erro ao solicitar geração do relatório de inscritos por formação. Filtros: {filtro}", LogContexto.Relatorio);
                return new Erro(TipoFalha.ErroInterno, "Erro ao solicitar relatório");
            }

            return Resultado.DeSucesso();
        }
    }
}
