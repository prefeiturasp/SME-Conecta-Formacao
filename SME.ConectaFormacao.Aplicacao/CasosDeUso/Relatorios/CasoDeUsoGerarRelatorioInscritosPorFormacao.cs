using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Relatorios;
using SME.ConectaFormacao.Aplicacao.Eventos.Relatorios;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios
{
    public class CasoDeUsoGerarRelatorioInscritosPorFormacao(
        IMediator mediator,
        IGeradorRelatorioInscritosExcelService geradorRelatorio,
        IRepositorioUsuario repositorioUsuario,
        IRepositorioRelatorios repositorioRelatorios,
        TimeProvider timeProvider,
        IServicoLogs servicoLogs) :
        CasoDeUsoAbstrato(mediator), ICasoDeUsoGerarRelatorioInscritosPorFormacao
    {
        private static readonly SemaphoreSlim _semaforo = new(initialCount: 2, maxCount: 2);
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var solicitacaoMensagem = param.ObterObjetoMensagem<SolicitacaoRelatorioInscritosPorFormacaoMensagem>();

            var resultadoValidacao = FiltroRelatorioInscritosValidador.Validar(solicitacaoMensagem.Filtros);
            if (!resultadoValidacao.Sucesso)
            {
                await servicoLogs.Enviar($"Validação dos filtros para geração do relatório de inscritos por formação falhou: {string.Join(", ", resultadoValidacao.MensagensErro)}",
                    LogContexto.Relatorio, LogNivel.Negocio);
                return false;
            }

            var dadosUsuario = await repositorioUsuario.ObterPorId(solicitacaoMensagem.Solicitante.UsuarioId);
            await _semaforo.WaitAsync();

            try
            {
                var dadosInscritos = await repositorioRelatorios.ObterDadosRelatorioInscritosPorFormacaoAsync(solicitacaoMensagem.Filtros);

                var inscritosFormatados = dadosInscritos.Select(MapearInscrito);

                var dadosRelatorio = new RelatorioInscritosFormacaoDto(
                    solicitacaoMensagem.Solicitante.Nome,
                    solicitacaoMensagem.Solicitante.Rf.AplicarMascaraCpf().AplicarMascaraRf(),
                    solicitacaoMensagem.DataSolicitacao,
                    inscritosFormatados
                );

                var urlAcessoRelatorio = await geradorRelatorio.GerarEArmazenarRelatorioAsync(dadosRelatorio);

                await mediator.Publish(new NotificarRelatorioEmitidoEvento(MontarNotificacao(urlAcessoRelatorio), [dadosUsuario!]));
                return true;
            }
            catch (Exception ex)
            {
                await servicoLogs.Enviar(ex, $"Erro ao gerar relatório de inscritos por formação", LogContexto.Relatorio, LogNivel.Critico);
                return false;
            }
            finally
            {
                _semaforo.Release();
            }
        }

        private NotificacaoDTO MontarNotificacao(string urlAcessoRelatorio)
        {
            return new NotificacaoDTO
            {

                Titulo = "Relatório de inscritos por formação (.xlsx)",
                Mensagem = MontarHtmlNotificacao(urlAcessoRelatorio),
                MensagemAposExpiracao = "O link para acesso ao relatório expirou. Por favor, solicite novamente se precisar acessar o relatório.",
                DataExpiracao = timeProvider.GetUtcNow().AddHours(24),
                Categoria = NotificacaoCategoria.Informe,
                Tipo = NotificacaoTipo.Relatorio
            };
        }
        private static string MontarHtmlNotificacao(string urlDownload)
        {
            return
                $"""
                <div style="font-family: Roboto, sans-serif; color: #42474A;">
                    <p style="font-weight: 400; font-size: 14px; line-height: 1.5; margin-bottom: 20px;">
                        O relatório de inscritos por formação está disponível, clique no botão "download" para baixar o arquivo.
                    </p>
                    <br/>
                    <a href="{urlDownload}" target="_blank" style="display: inline-flex; align-items: center; justify-content: center; width: 127px; height: 38px; background-color: #FF9A52; color: #FFFFFF; font-weight: 700; font-size: 14px; text-decoration: none; border-radius: 4px;">
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg" style="margin-right: 8px;">
                            <path model="M14 11V14H2V11H0V14C0 15.1 0.9 16 2 16H14C15.1 16 16 15.1 16 14V11H14ZM13 7L11.59 5.59L9 8.17V0H7V8.17L4.41 5.59L3 7L8 12L13 7Z" fill="white"/>
                        </svg>
                        Download
                    </a>                
                    <br/>
                    <p style="font-weight: 400; font-size: 14px; line-height: 1.5; margin-top: 20px;">
                        Observação: O Download deve ser realizado em até 24 horas, após este prazo o arquivo será excluído e caso necessite você deve solicitar um novo relatório.
                    </p>
                </div>
                """;
        }

        private static InscritoFormacaoDto MapearInscrito(InscritoFormacaoQueryModel model) =>
            new(model.CodigoFormacao,
                model.CodigoHomologacao,
                model.NomeFormacao,
                model.AreaPromotora,
                model.Dre ?? "N/A",
                model.Ue ?? "N/A",
                Periodo: FormatarDataPeriodo(model.DataRealizacaoInicio, model.DataRealizacaoFim) ?? "N/A",
                model.SituacaoFormacao?.Nome() ?? "N/A",
                model.ModalidadeFormativa?.Nome() ?? "N/A",
                model.PublicoAlvo ?? "N/A",
                model.FuncaoEspecifica ?? "N/A",
                model.EtapaModalidade?.Nome() ?? "N/A",
                model.AnoEtapa ?? "N/A",
                model.ComponenteCurricular ?? "N/A",
                model.Turma ?? "N/A",
                model.RfCpf?.AplicarMascaraCpf().AplicarMascaraRf() ?? "N/A",
                model.NomeCursista ?? "N/A",
                model.SituacaoInscricao?.Nome() ?? "N/A",
                model.SituacaoConclusaoCursista ?? "N/A",
                model.Email ?? "N/A",
                FormatarBoolean(model.Pcd),
                model.DescricaoDeficiencia ?? "N/A",
                model.Pcd.HasValue && model.Pcd.Value ? FormatarBoolean(model.NecessitaAdaptacao) : "",
                model.Pcd.HasValue && model.Pcd.Value ? model.DescricaoAdaptacao ?? "N/A" : "");

        private static string? FormatarDataPeriodo(DateTime? inicio, DateTime? fim)
        {
            if (!inicio.HasValue || !fim.HasValue) return null;

            return $"{inicio.Value:dd/MM/yyyy} À {fim.Value:dd/MM/yyyy}";
        }

        private static string FormatarBoolean(bool? valor)
        {
            if (!valor.HasValue) return "N/A";
            return valor.Value ? "Sim" : "Não";
        }
    }
}