using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioDre : RepositorioBaseAuditavel<Dre>, IRepositorioDre
    {
        public RepositorioDre(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<bool> VerificarSeDreExistePorCodigo(string codigoDre)
        {
            var query = @"select count(1) from dre d  where d.dre_id  = @codigoDre and not excluido";
            return await conexao.Obter().ExecuteScalarAsync<bool>(query, new { codigoDre });
        }
        public Task AtualizarDreComEol(Dre dre)
        {
            PreencherAuditoriaAlteracao(dre);

            var parametros = new
            {
                dre.Id,
                dre.Nome,
                dre.Abreviacao,
                DataAtualizacao = DateTime.Now,
                dre.AlteradoEm,
                dre.AlteradoPor,
                dre.AlteradoLogin
            };

            var query = @"update
	                        public.dre
                        set
	                        abreviacao = @Abreviacao,
	                        nome = @Nome,
	                        data_atualizacao = @DataAtualizacao,
	                        alterado_em = @AlteradoEm,
	                        alterado_por = @AlteradoPor,
	                        alterado_login = @AlteradoLogin
                        where id = @Id";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }
        public async Task<Dre> ObterDrePorCodigo(string codigoDre)
        {

            var query = @"select
	                            d.id,
	                            d.dre_id,
	                            d.abreviacao,
	                            d.nome,
	                            d.data_atualizacao,
	                            d.criado_em,
	                            d.criado_por,
	                            d.alterado_em,
	                            d.alterado_por,
	                            d.criado_login,
	                            d.alterado_login,
	                            d.excluido
                            from
	                            public.dre d  
                            where d.dre_id  = @codigoDre ";
            return await conexao.Obter().QueryFirstOrDefaultAsync<Dre>(query, new { codigoDre });
        }
    }
}