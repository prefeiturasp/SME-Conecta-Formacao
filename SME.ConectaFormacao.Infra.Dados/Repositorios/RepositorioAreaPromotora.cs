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

        public async Task<AreaPromotora?> ObterAreaPromotoraPorIdDetalhadoAsync(long areaPromotoraId)
        {
            var query =
                """
                select ap.*, d.*, c.*
                from area_promotora ap
                left join dre d on ap.dreid = d.id
                left join coordenadoria c on ap.coordenadoria_id = c.id and not c.excluido
                where not ap.excluido and ap.id = @areaPromotoraId
                """;

            return (await conexao.Obter().QueryAsync<AreaPromotora, Dre, Coordenadoria, AreaPromotora>(query, (areaPromotora, dre, coordenadoria) =>
            {
                areaPromotora.AdicionarDre(dre);
                areaPromotora.Coordenadoria = coordenadoria;
                return areaPromotora;
            }, new { areaPromotoraId })).FirstOrDefault();

        }

        public Task<IEnumerable<AreaPromotora>> ObterDadosPaginados(string nome, short? tipo, long? coordenadoriaId, int numeroPagina, int numeroRegistros)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            string query = MontarQueryListagem(ref nome, tipo, coordenadoriaId);

            query += " order by ap.nome, c.nome";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<AreaPromotora, Dre, Coordenadoria, AreaPromotora>(query, (areaPromotora, dre, coordenadoria) =>
            {
                areaPromotora.Dre = dre;
                areaPromotora.Coordenadoria = coordenadoria;
                return areaPromotora;
            }, new { nome, tipo, coordenadoriaId, numeroRegistros, registrosIgnorados });
        }

        public Task<int> ObterTotalRegistrosPorFiltros(string nome, short? tipo, long? coordenadoriaId)
        {
            string query = string.Concat("select count(1) from (", MontarQueryListagem(ref nome, tipo, coordenadoriaId), ") tb");

            return conexao.Obter().ExecuteScalarAsync<int>(query, new { nome, tipo, coordenadoriaId });
        }

        private static string MontarQueryListagem(ref string nome, short? tipo, long? coordenadoriaId)
        {
            var query =
                """
                select ap.*, d.*, c. *
                from area_promotora ap
                     left join dre d  on ap.dreid = d.id
                     left join coordenadoria c on ap.coordenadoria_id = c.id and not c.excluido
                where not ap.excluido
                """;

            if (!string.IsNullOrEmpty(nome))
            {
                nome = "%" + nome.ToLower() + "%";
                query += $" and lower(ap.nome) like @nome";
            }

            if (tipo.GetValueOrDefault() > 0)
                query += " and ap.tipo = @tipo";

            if (coordenadoriaId is not null)
                query += " and ap.coordenadoria_id = @coordenadoriaId";

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
            var query = @"select id, nome, grupo_id, tipo from area_promotora where not excluido order by nome";

            return conexao.Obter().QueryAsync<AreaPromotora>(query);
        }

        public Task<bool> ExistePropostaPorId(long id)
        {
            var query = @"select 1 from proposta where not excluido and area_promotora_id = @id limit 1 ";
            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { id });
        }

        public Task<AreaPromotora> ObterAreaPromotoraPorPropostaId(long propostaId)
        {
            var query = @" 
            select 
               id,
               nome,
               tipo,
               email,
               grupo_id,
               excluido,
               criado_em,
               criado_por,
               alterado_em,
               alterado_por,
               criado_login,
               alterado_login,
               dreid
            from area_promotora ap
            where not excluido and exists(select 1 from proposta p where not p.excluido and ap.id = p.area_promotora_id and p.id = @propostaId)";

            return conexao.Obter().QueryFirstOrDefaultAsync<AreaPromotora>(query, new { propostaId });
        }
    }
}