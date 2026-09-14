using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoGerarRelatorioCodafCursoNaoHomologado(
        IRepositorioCodafCursoNaoHomologado repositorioCodafCursoNaoHomologado,
        IGeradorRelatorioCodafCursoNaoHomologadoExcelService geradorRelatorioExcelService,
        IContextoAplicacao contextoAplicacao) :
        ICasoDeUsoGerarRelatorioCodafCursoNaoHomologado
    {
        private const int STATUS_DECLARACAO_EMITIDA = 4;

        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long id)
        {
            var codaf = await repositorioCodafCursoNaoHomologado.ObterPorIdDetalhadoAsync(id);
            if (codaf == null)
                return Erro.NaoEncontrado();

            if (!contextoAplicacao.EhAdministrador && codaf.CriadoLogin != contextoAplicacao.UsuarioLogado)
                return Erro.Negocio("Você não tem permissão para gerar relatório deste registro.");

            var statusDeclaracao = await repositorioCodafCursoNaoHomologado.ObterStatusDeclaracaoTurmaAsync(id);
            if (statusDeclaracao != STATUS_DECLARACAO_EMITIDA)
                return Erro.Negocio("As declarações desta turma ainda não foram emitidas.");

            var dadosRelatorio = await repositorioCodafCursoNaoHomologado.ObterDadosRelatorioAsync(id);
            if (dadosRelatorio == null)
                return Erro.NaoEncontrado("Nenhuma informação encontrada para o codaf informado.");

            var arquivoBytes = geradorRelatorioExcelService.GerarRelatorio(dadosRelatorio);

            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var nomeArquivo = $"CODAF_{codaf.PropostaId}_{codaf.PropostaTurma.Nome}.xlsx";
            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, new MemoryStream(arquivoBytes, writable: false));

            codaf.Finalizar();
            await repositorioCodafCursoNaoHomologado.Atualizar(codaf);

            return arquivoDto;
        }
    }
}