using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricao;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioImportacaoArquivo : RepositorioBaseAuditavel<ImportacaoArquivo>, IRepositorioImportacaoArquivo
    {
        public RepositorioImportacaoArquivo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<RegistrosPaginados<ArquivosImportadosTotalRegistro>> ObterArquivosInscricaoImportacao(int quantidadeRegistroIgnorados, int numeroRegistros, long propostaId)
        {
            var sql = new StringBuilder();

            sql.AppendLine(@"with TotalRegistro as (
                            select count (id)TotalRegistro, importacao_arquivo_id
                            from importacao_arquivo_registro
                            where not excluido
                            group by importacao_arquivo_id
                            ),");
            sql.AppendLine(@"TotalProcessados as (
                            select count (id)TotalProcessados, importacao_arquivo_id
                            from importacao_arquivo_registro
                            where situacao = @situacaoProcessado
                              and not excluido
                            group by importacao_arquivo_id
                            )");
            sql.AppendLine(@" select id, nome, situacao, 
                                    coalesce(tr.TotalRegistro, 0) as TotalRegistros, 
                                    coalesce(tp.TotalProcessados, 0) as TotalProcessados
                              from importacao_arquivo ia
                              left join TotalRegistro tr on tr.importacao_arquivo_id = ia.id
                              left join TotalProcessados tp on tp.importacao_arquivo_id = ia.id
                              where proposta_id = @propostaId
                                and not excluido
                              order by criado_em desc");

            sql.AppendLine($" OFFSET {quantidadeRegistroIgnorados} ROWS FETCH NEXT {numeroRegistros} ROWS ONLY; ");

            sql.AppendLine(@"select count(id)
                             from importacao_arquivo
                             where proposta_id = @propostaId
                               and not excluido;");

            var parametros = new { propostaId, situacaoProcessado = SituacaoImportacaoArquivoRegistro.Processado };

            var retorno = new RegistrosPaginados<ArquivosImportadosTotalRegistro>();

            using (var multi = await conexao.Obter().QueryMultipleAsync(sql.ToString(), parametros))
            {
                retorno.Registros = multi.Read<ArquivosImportadosTotalRegistro>();
                retorno.TotalRegistros = multi.ReadFirst<int>();
            }

            return retorno;
        }
    }
}
