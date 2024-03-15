using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioImportacaoArquivoRegistro : RepositorioBaseAuditavel<ImportacaoArquivoRegistro>, IRepositorioImportacaoArquivoRegistro
    {
        public RepositorioImportacaoArquivoRegistro(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<IEnumerable<ImportacaoArquivoRegistro>> ObterRegistrosImportacaoArquivoInscricaoCursistasPaginados(long importacaoArquivoId, int numeroRegistros, int quantidadeRegistroIgnorados)
        {
            var sql = @"            
                SELECT id,
                  iar.importacao_arquivo_id,
                  linha,
                  conteudo,
                  situacao,
                  erro,
                  criado_em,
                  criado_por,
                  criado_login,
                  alterado_em,
                  alterado_por,
                  alterado_login,
                  excluido,
                  coalesce(tr.TotalRegistro, 0) as TotalRegistros
            FROM importacao_arquivo_registro iar
            LEFT JOIN TotalRegistros tr on tr.importacao_arquivo_id = iar.importacao_arquivo_id
            WHERE iar.importacao_arquivo_id = @importacaoArquivoId
            AND not excluido
            OFFSET {quantidadeRegistroIgnorados} ROWS FETCH NEXT {numeroRegistros} ROWS ONLY ";
            
            return await conexao.Obter().QueryAsync<ImportacaoArquivoRegistro>(sql, new { importacaoArquivoId });
        }
    }
}
