using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioImportacaoArquivo : RepositorioBaseAuditavel<ImportacaoArquivo>, IRepositorioImportacaoArquivo
    {
        public RepositorioImportacaoArquivo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<RegistrosPaginados<ArquivosImportadosTotalRegistro>> ObterArquivosInscricaoImportacao(int quantidadeRegistroIgnorados, int numeroRegistros, long propostaId)
        {
            var sql = @$"
                    select ia.id,
	                       ia.nome,
	                       ia.situacao,
	                       ia.criado_em,
	                       count(iar.id) as TotalRegistros,
	                       sum(
		                       case 
	   		                    when (ia.situacao = {(int)SituacaoImportacaoArquivo.Validado} or ia.situacao = {(int)SituacaoImportacaoArquivo.Validando}) and iar.situacao = {(int)SituacaoImportacaoArquivoRegistro.Validado} then 1
	   		                    when (ia.situacao = {(int)SituacaoImportacaoArquivo.Processado} or ia.situacao = {(int)SituacaoImportacaoArquivo.Processando}) and iar.situacao = {(int)SituacaoImportacaoArquivoRegistro.Processado} then 1
	   		                    when ia.situacao = {(int)SituacaoImportacaoArquivo.Cancelado} then 1
	   		                    when iar.situacao = {(int)SituacaoImportacaoArquivoRegistro.Erro} then 1
		   	                    else 0
		                       end
	                       ) as TotalProcessados
                    from importacao_arquivo ia
                    left join importacao_arquivo_registro iar on iar.importacao_arquivo_id = ia.id and not iar.excluido
                    where ia.proposta_id = @propostaId
                      and not ia.excluido
                    group by ia.id, ia.nome, ia.situacao, ia.criado_em
                    order by ia.criado_em desc ";

            sql += $" OFFSET {quantidadeRegistroIgnorados} ROWS FETCH NEXT {numeroRegistros} ROWS ONLY; ";

            sql += "select count(id) from importacao_arquivo where proposta_id = @propostaId and not excluido;";

            var parametros = new { propostaId };

            var retorno = new RegistrosPaginados<ArquivosImportadosTotalRegistro>();

            using (var multi = await conexao.Obter().QueryMultipleAsync(sql.ToString(), parametros))
            {
                retorno.Registros = await multi.ReadAsync<ArquivosImportadosTotalRegistro>();
                retorno.TotalRegistros = await multi.ReadFirstAsync<int>();
            }

            return retorno;
        }
    }
}
