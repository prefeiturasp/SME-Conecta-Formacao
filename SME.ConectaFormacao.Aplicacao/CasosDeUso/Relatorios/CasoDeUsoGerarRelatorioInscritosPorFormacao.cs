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

                    <a href="{urlDownload}" target="_blank"
                    style="display: inline-flex; align-items: center; justify-content: center; gap: 8px; width: 127px; height: 38px; background-color: #FF9A52; color: #FFFFFF; font-weight: 700; font-size: 14px; text-decoration: none; border-radius: 4px;">
                        
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M8 11.575C7.86667 11.575 7.74167 11.5543 7.625 11.513C7.50833 11.4717 7.4 11.4007 7.3 11.3L3.7 7.7C3.5 7.5 3.404 7.26667 3.412 7C3.42 6.73334 3.516 6.5 3.7 6.3C3.9 6.1 4.13767 5.996 4.413 5.988C4.68833 5.98 4.92567 6.07567 5.125 6.275L7 8.15V1C7 0.71667 7.096 0.479337 7.288 0.288004C7.48 0.0966702 7.71733 0.000670115 8 3.44827e-06C8.28267 -0.000663218 8.52033 0.0953369 8.713 0.288004C8.90567 0.48067 9.00133 0.718003 9 1V8.15L10.875 6.275C11.075 6.075 11.3127 5.979 11.588 5.987C11.8633 5.995 12.1007 6.09934 12.3 6.3C12.4833 6.5 12.5793 6.73334 12.588 7C12.5967 7.26667 12.5007 7.5 12.3 7.7L8.7 11.3C8.6 11.4 8.49167 11.471 8.375 11.513C8.25833 11.555 8.13333 11.5757 8 11.575ZM2 16C1.45 16 0.979333 15.8043 0.588 15.413C0.196666 15.0217 0.000666667 14.5507 0 14V12C0 11.7167 0.0960001 11.4793 0.288 11.288C0.48 11.0967 0.717333 11.0007 1 11C1.28267 10.9993 1.52033 11.0953 1.713 11.288C1.90567 11.4807 2.00133 11.718 2 12V14H14V12C14 11.7167 14.096 11.4793 14.288 11.288C14.48 11.0967 14.7173 11.0007 15 11C15.2827 10.9993 15.5203 11.0953 15.713 11.288C15.9057 11.4807 16.0013 11.718 16 12V14C16 14.55 15.8043 15.021 15.413 15.413C15.0217 15.805 14.5507 16.0007 14 16H2Z" fill="white"/>
                        </svg>

                        Download
                    </a>

                    <br/><br/>

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