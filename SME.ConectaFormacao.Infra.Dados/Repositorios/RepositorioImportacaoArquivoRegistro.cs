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

        public async Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosComErro(int quantidadeRegistroIgnorados, int numeroRegistros, long importacaoArquivoId)
        {
            return await ObterRegistroPorSituacao(quantidadeRegistroIgnorados, numeroRegistros, importacaoArquivoId, SituacaoImportacaoArquivoRegistro.Erro);
        }

        public async Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistrosValidados(int quantidadeRegistroIgnorados, int numeroRegistros, long importacaoArquivoId)
        {
            return await ObterRegistroPorSituacao(quantidadeRegistroIgnorados, numeroRegistros, importacaoArquivoId, SituacaoImportacaoArquivoRegistro.Validado);
        }

        public async Task<RegistrosPaginados<ImportacaoArquivoRegistro>> ObterRegistroPorSituacao(int quantidadeRegistroIgnorados, int numeroRegistros, long importacaoArquivoId, SituacaoImportacaoArquivoRegistro situacao)
        {
            var sql = new StringBuilder();

            sql.AppendLine(@" SELECT id,
                                     importacao_arquivo_id,
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
                                     excluido
                              FROM importacao_arquivo_registro
                              WHERE importacao_arquivo_id = @importacaoArquivoId
                                AND situacao = @situacao
                              ORDER BY linha");

            sql.AppendLine($" OFFSET {quantidadeRegistroIgnorados} ROWS FETCH NEXT {numeroRegistros} ROWS ONLY; ");

            sql.AppendLine(@"select count(id)
                             from importacao_arquivo_registro
                             where importacao_arquivo_id = @importacaoArquivoId
                               and situacao = @situacao;");

            var parametros = new { importacaoArquivoId, situacao };

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
