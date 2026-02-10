using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCodafLogRemessaConclusao(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) : IRepositorioCodafLogRemessaConclusao
    {
        public async Task InserirAsync(CodafLogRemessaConclusao codafLogRemessaConclusao)
        {
            codafLogRemessaConclusao.DataGeracao = DateTime.Now;
            codafLogRemessaConclusao.CriadoLogin = contexto.UsuarioLogado;
            const string sql = """
                INSERT INTO public.codaf_log_remessa_conclusao 
                (
                    codaf_lista_presenca_id, 
                    criado_login, 
                    data_geracao, 
                    hash_arquivo, 
                    quantidade_registros, 
                    nome_arquivo_gerado
                )
                VALUES 
                (
                    @CodafListaPresencaId, 
                    @CriadoLogin, 
                    @DataGeracao, 
                    @HashArquivo, 
                    @QuantidadeRegistros, 
                    @NomeArquivoGerado
                );
                """;
            var conn = conexao.Obter();
            await conn.ExecuteAsync(sql, codafLogRemessaConclusao);
        }
    }
}
