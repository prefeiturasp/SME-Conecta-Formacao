using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioImportacaoArquivoRegistro : RepositorioBaseAuditavel<ImportacaoArquivoRegistro>, IRepositorioImportacaoArquivoRegistro
    {
        public RepositorioImportacaoArquivoRegistro(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosComErro(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId)
        {
            return await ObterRegistroPorSituacao(quantidadeRegistroIgnorados, numeroRegistros, arquivoId, SituacaoImportacaoArquivoRegistro.Erro);
        }

        public async Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosValidados(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId)
        {
            return await ObterRegistroPorSituacao(quantidadeRegistroIgnorados, numeroRegistros, arquivoId, SituacaoImportacaoArquivoRegistro.Validado);
        }

        private async Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistroPorSituacao(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId, SituacaoImportacaoArquivoRegistro situacao)
        {
            var sql = new StringBuilder();

            sql.AppendLine(@" select linha, conteudo, erro
                              from importacao_arquivo_registro
                              where importacao_arquivo_id = @arquivoId
                                and not excluido
                                and situacao = @situacao
                              order by linha");

            sql.AppendLine($" OFFSET {quantidadeRegistroIgnorados} ROWS FETCH NEXT {numeroRegistros} ROWS ONLY; ");

            sql.AppendLine(@"select count(id)
                             from importacao_arquivo_registro
                             where importacao_arquivo_id = @arquivoId
                               and not excluido
                               and situacao = @situacao;");

            var parametros = new { arquivoId, situacao };

            var retorno = new RegistrosPaginados<ImportacaoArquivoRegistro>();

            using (var multi = await conexao.Obter().QueryMultipleAsync(sql.ToString(), parametros))
            {
                retorno.Registros = multi.Read<ImportacaoArquivoRegistro>();
                retorno.TotalRegistros = multi.ReadFirst<int>();
            }

            return retorno;
        }
    }
}
