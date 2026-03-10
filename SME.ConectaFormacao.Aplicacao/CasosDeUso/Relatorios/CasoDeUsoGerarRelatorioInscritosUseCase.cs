using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios
{
    public class CasoDeUsoGerarRelatorioInscritosUseCase(IMediator mediator, IGeradorRelatorioInscritosExcelService geradorRelatorio) :
        CasoDeUsoAbstrato(mediator), ICasoDeUsoGerarRelatorioInscritosUseCase
    {
        private static readonly SemaphoreSlim _semaforo = new(initialCount: 2, maxCount: 2);
        public async Task<bool> Executar(MensagemRabbit param)
        {
            // 1. Aguarda na fila do semáforo se já houverem 2 processamentos em andamento
            await _semaforo.WaitAsync();

            try
            {
                // 2. Busca os dados no

                // 3. Transforma os dados para o formato do relatório

                // 4. Gera o relatório e armazena
                await geradorRelatorio.GerarEArmazenarRelatorioAsync(MockTemporario);
                return true;
            }
            finally
            {
                _semaforo.Release();
            }
        }

        private static RelatorioInscritosFormacaoDto MockTemporario => new(        
            "Diego Ferreira Moreno",
            "098.111.206-41",
            DateTime.Now,
            [new("245", "25050", "EMERGÊNCIA CLIMÁTICA: O PAPEL DA EDUCAÇÃO E A INTERLOCUÇÃO COM A FORMAÇÃO DOCENTE",
                "SINPEEM", "DRE IPIRANGA", "EMEI MARIA LUIZA MORETTI GENTILE, PROFA.", "21/03/2026 À 12/04/2026", "Publicada", "À DISTÂNCIA", "PROF. ED. INF. E ENS. FUND. I",
                "Professor Principal", "Ensino Fundamental", "Todos", "Educação Física", "EI - 7E - INF", "710.886.9", "MARCIA APARECIDA GORGATTI CARLOS ANTONIO",
                "Iniciado", "Iniciado", "marcia.antonio@sme.prefeitura.sp.gov.br", "Não", "", "", ""),
            new("245", "25050", "EMERGÊNCIA CLIMÁTICA: O PAPEL DA EDUCAÇÃO E A INTERLOCUÇÃO COM A FORMAÇÃO DOCENTE",
                "SINPEEM", "DRE IPIRANGA", "EMEI MONTESE.", "21/03/2026 À 12/04/2026", "Publicada", "À DISTÂNCIA", "PROF. ED. INF. E ENS. FUND. I",
                "Professor Principal", "Ensino Fundamental", "Todos", "Educação Física", "EI - 7B - INF", "794.360.1", "FABIO RODRIGUES LEMES",
                "Iniciado", "Iniciado", "fabiorlkafka@gmail.com", "Não", "", "", ""),
            new("246", "25153", "INTERSECCIONALIDADES: GÊNERO, RAÇA E EDUCAÇÃO EM PERSPECTIVA CRÍTICA",
                "DRE PE", "DRE PENHA", "EMEI MONTESE.", "20/03/2026 À 10/04/2026", "Publicada", "Hibrído", "PROF. ED. INF. E ENS. FUND. I",
                "Professor Principal", "Ensino Fundamental", "Todos", "Educação Física", "EI - 7B - INF", "794.360.1", "MARTA AUGUSTO FERREIRA CAVICHIOLLI",
                "Iniciado", "Iniciado", "fabiorlkafka@gmail.com", "Não", "", "", "")]
        );
    }
}