using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoGerarRelatorioCodaf(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IGeradorRelatorioCodafExcelService geradorRelatorioCodafSuplementarExcelService,
        IContextoAplicacao contextoAplicacao) :
        ICasoDeUsoGerarRelatorioCodaf
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafId)
        {
            bool perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var listaPresenca = await repositorioCodafListaPresenca.ObterPorIdComPropostaEPropostaTurmaAsync(codafId);
            if (listaPresenca == null)
                return Erro.NaoEncontrado();

            if (perfilRestrito && listaPresenca.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para gerar relatório desta lista de presença.");

            var dadosRelatorio = await repositorioCodafListaPresenca.ObterDadosRelatorioAsync(codafId);
            if (dadosRelatorio == null)
                return Erro.NaoEncontrado("Nenhuma informação encontrada para o codaf informado.");

            var arquivoBytes = geradorRelatorioCodafSuplementarExcelService.GerarRelatorio(dadosRelatorio, ehCodafSuplementar: false);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var nomeArquivo = $"CODAF_{listaPresenca.Proposta.NumeroHomologacao}-{listaPresenca.PropostaTurma.Nome}.xlsx";
            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, new MemoryStream(arquivoBytes, writable: false));
            await AtualizarStatusParaFinalizadoAsync(listaPresenca);
            return arquivoDto;
        }

        private async Task AtualizarStatusParaFinalizadoAsync(CodafListaPresenca listaPresenca)
        {
            if (listaPresenca.Status == StatusCodafListaPresenca.Finalizado)
                return;

            listaPresenca.Finalizar();
            await repositorioCodafListaPresenca.Atualizar(listaPresenca);

        }
    }
}
