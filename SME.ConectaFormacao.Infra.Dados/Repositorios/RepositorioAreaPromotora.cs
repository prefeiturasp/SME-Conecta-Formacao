using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioAreaPromotora : RepositorioBaseAuditavel<AreaPromotora>, IRepositorioAreaPromotora
    {
        public RepositorioAreaPromotora(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<AreaPromotora?> ObterAreaPromotoraPorIdComDre(long areaPromotoraId)
        {
            var query = @" select ap.*, d.*
                             from area_promotora ap
                             left join dre d on ap.dreid = d.id
                            where not ap.excluido and ap.id = @areaPromotoraId ";

            return (await conexao.Obter().QueryAsync<AreaPromotora, Dre, AreaPromotora>(query, (areaPromotora, dre) =>
            {
                areaPromotora.AdicionarDre(dre);
                return areaPromotora;
            }, new { areaPromotoraId })).FirstOrDefault();
        }

        public Task<IEnumerable<AreaPromotora>> ObterDadosPaginados(string nome, short? tipo, int numeroPagina, int numeroRegistros)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            string query = MontarQueryListagem(ref nome, tipo);

            query += " order by ap.nome";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<AreaPromotora, Dre, AreaPromotora>(query, (areaPromotora, dre) =>
            {
                areaPromotora.Dre = dre;
                return areaPromotora;
            }, new { nome, tipo, numeroRegistros, registrosIgnorados });
        }

        public Task<int> ObterTotalRegistrosPorFiltros(string nome, short? tipo)
        {
            string query = string.Concat("select count(1) from (", MontarQueryListagem(ref nome, tipo), ") tb");

            return conexao.Obter().ExecuteScalarAsync<int>(query, new { nome, tipo });
        }

        private static string MontarQueryListagem(ref string nome, short? tipo)
        {
            var query = @"select ap.*, d.*
                          from area_promotora ap
                          left join dre d  on ap.dreid = d.id 
                          where not ap.excluido ";

            if (!string.IsNullOrEmpty(nome))
            {
                nome = "%" + nome.ToLower() + "%";
                query += $" and lower(ap.nome) like @nome";
            }

            if (tipo.GetValueOrDefault() > 0)
                query += " and ap.tipo = @tipo";

            return query;
        }

        public async Task<long> Inserir(IDbTransaction transacao, AreaPromotora areaPromotora)
        {
            PreencherAuditoriaCriacao(areaPromotora);

            areaPromotora.Id = (long)await conexao.Obter().InsertAsync(areaPromotora, transacao);
            return areaPromotora.Id;
        }

        public Task<bool> Atualizar(IDbTransaction transacao, AreaPromotora areaPromotora)
        {
            PreencherAuditoriaAlteracao(areaPromotora);

            return conexao.Obter().UpdateAsync(areaPromotora, transacao);
        }

        public Task<bool> Remover(IDbTransaction transacao, AreaPromotora areaPromotora)
        {
            PreencherAuditoriaAlteracao(areaPromotora);

            areaPromotora.Excluido = true;

            return conexao.Obter().UpdateAsync(areaPromotora, transacao);
        }

        public Task<IEnumerable<AreaPromotoraTelefone>> ObterTelefonesPorId(long id)
        {
            var query = @"select 
                            id, 
                            area_promotora_id, 
                            telefone,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from area_promotora_telefone 
                        where not excluido and area_promotora_id = @id";

            return conexao.Obter().QueryAsync<AreaPromotoraTelefone>(query, new { id });
        }

        public async Task InserirTelefones(IDbTransaction transacao, long id, IEnumerable<AreaPromotoraTelefone> telefones)
        {
            foreach (var telefone in telefones)
            {
                PreencherAuditoriaCriacao(telefone);

                telefone.AreaPromotoraId = id;
                telefone.Id = (long)await conexao.Obter().InsertAsync(telefone, transacao);
            }
        }

        public async Task RemoverTelefones(IDbTransaction transacao, long id, IEnumerable<AreaPromotoraTelefone> telefones)
        {
            foreach (var telefone in telefones)
            {
                PreencherAuditoriaAlteracao(telefone);

                telefone.Excluido = true;
                await conexao.Obter().UpdateAsync(telefone, transacao);
            }
        }

        public Task<bool> ExistePorGrupoId(Guid grupoId, long ignorarAreaPromotoraId)
        {
            var query = @"select count(1) from area_promotora where grupo_id = @grupoId and not excluido";

            if (ignorarAreaPromotoraId > 0)
                query += " and id <> @ignorarAreaPromotoraId";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { grupoId, ignorarAreaPromotoraId });
        }

        public Task<bool> ExistePorGrupoIdEDreId(long dreId, Guid grupoId, long ignorarAreaPromotoraId)
        {
            var query = @"select count(1) from area_promotora 
                            where grupo_id = @grupoId 
                            and dreid  = @dreId
                            and not excluido";

            if (ignorarAreaPromotoraId > 0)
                query += " and id <> @ignorarAreaPromotoraId";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { grupoId, dreId, ignorarAreaPromotoraId });
        }

        public Task<AreaPromotora> ObterPorGrupoIdEDres(Guid grupoId, string[] dres)
        {
            var query = @"
                    select ap.id, ap.nome, ap.tipo, ap.email, ap.dreid  
                    from area_promotora ap
                    left join dre d on d.id = ap.dreid and not d.excluido  
                    where ap.grupo_id = @grupoId 
                      and not ap.excluido ";

            if (dres.PossuiElementos())
                query += " and d.dre_id = any(@dres) ";

            query += " limit 1";

            return conexao.Obter().QueryFirstOrDefaultAsync<AreaPromotora>(query, new { grupoId, dres });
        }

        public Task<IEnumerable<AreaPromotora>> ObterLista()
        {
            var query = @"select id, nome, grupo_id  from area_promotora where not excluido order by nome";

            return conexao.Obter().QueryAsync<AreaPromotora>(query);
        }

        public Task<bool> ExistePropostaPorId(long id)
        {
            var query = @"select 1 from proposta where not excluido and area_promotora_id = @id limit 1 ";
            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { id });
        }
    }
}