using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCodafSuplementarLogRemessaConclusao(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) : IRepositorioCodafSuplementarLogRemessaConclusao
    {
        public async Task InserirAsync(CodafSuplementarLogRemessaConclusao codafSuplementarLogRemessaConclusao)
        {
            codafSuplementarLogRemessaConclusao.DataGeracao = DateTime.Now;
            codafSuplementarLogRemessaConclusao.CriadoLogin = contexto.UsuarioLogado;
            const string sql = """
                INSERT INTO public.codaf_suplementar_log_remessa_conclusao 
                (
                    codaf_suplementar_id, 
                    criado_login, 
                    data_geracao, 
                    hash_arquivo, 
                    quantidade_registros, 
                    nome_arquivo_gerado
                )
                VALUES 
                (
                    @CodafSuplementarId, 
                    @CriadoLogin, 
                    @DataGeracao, 
                    @HashArquivo, 
                    @QuantidadeRegistros, 
                    @NomeArquivoGerado
                );
                """;
            var conn = conexao.Obter();
            await conn.ExecuteAsync(sql, codafSuplementarLogRemessaConclusao);
        }
    }
}
