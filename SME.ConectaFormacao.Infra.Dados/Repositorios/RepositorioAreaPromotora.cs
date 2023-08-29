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

        public Task<IEnumerable<AreaPromotora>> ObterDadosPaginados(string nome, short? tipo, int numeroPagina, int numeroRegistros)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            string query = MontarQueryListagem(nome, tipo);

            if (!string.IsNullOrEmpty(nome))
                nome = "%" + nome + "%";

            query += " order by nome";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<AreaPromotora>(query, new { nome, tipo, numeroRegistros, registrosIgnorados });
        }

        public Task<int> ObterTotalRegistrosPorFiltros(string nome, short? tipo)
        {
            string query = string.Concat("select count(1) from (", MontarQueryListagem(nome, tipo), ") tb");
            return conexao.Obter().ExecuteScalarAsync<int>(query, new { nome, tipo });
        }

        private static string MontarQueryListagem(string nome, short? tipo)
        {
            var query = @"select id, nome, tipo
                          from area_promotora
                          where not excluido ";

            if (!string.IsNullOrEmpty(nome))
                query += " and nome like @nome";

            if (tipo.GetValueOrDefault() > 0)
                query += " and tipo = @tipo";

            return query;
        }

        public async Task<long> Inserir(IDbTransaction transacao, AreaPromotora areaPromotora)
        {
            areaPromotora.CriadoEm = DateTimeExtension.HorarioBrasilia();
            areaPromotora.CriadoPor = contexto.NomeUsuario;
            areaPromotora.CriadoLogin = contexto.UsuarioLogado;
            areaPromotora.Id = (long)await conexao.Obter().InsertAsync(areaPromotora, transacao);
            return areaPromotora.Id;
        }

        public Task<bool> Atualizar(IDbTransaction transacao, AreaPromotora areaPromotora)
        {
            areaPromotora.AlteradoEm = DateTimeExtension.HorarioBrasilia();
            areaPromotora.AlteradoPor = contexto.NomeUsuario;
            areaPromotora.AlteradoLogin = contexto.UsuarioLogado;

            return conexao.Obter().UpdateAsync(areaPromotora, transacao);
        }

        public Task<bool> Remover(IDbTransaction transacao, AreaPromotora areaPromotora)
        {
            areaPromotora.AlteradoEm = DateTimeExtension.HorarioBrasilia();
            areaPromotora.AlteradoPor = contexto.NomeUsuario;
            areaPromotora.AlteradoLogin = contexto.UsuarioLogado;
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
                telefone.CriadoEm = DateTimeExtension.HorarioBrasilia();
                telefone.CriadoPor = contexto.NomeUsuario;
                telefone.CriadoLogin = contexto.UsuarioLogado;
                telefone.AreaPromotoraId = id;
                telefone.Id = (long)await conexao.Obter().InsertAsync(telefone, transacao);
            }
        }

        public async Task RemoverTelefones(IDbTransaction transacao, long id, IEnumerable<AreaPromotoraTelefone> telefones)
        {
            foreach (var telefone in telefones)
            {
                telefone.AlteradoEm = DateTimeExtension.HorarioBrasilia();
                telefone.AlteradoPor = contexto.NomeUsuario;
                telefone.AlteradoLogin = contexto.UsuarioLogado;
                telefone.Excluido = true;
                await conexao.Obter().UpdateAsync(telefone, transacao);
            }
        }
    }
}
