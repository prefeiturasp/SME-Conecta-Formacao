using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioUsuario : RepositorioBaseAuditavel<Usuario>, IRepositorioUsuario
    {
        public RepositorioUsuario(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<Usuario> ObterPorLogin(string login)
        {
            var query = @"select 
                            id, 
                            login,                             
                            nome,       
                            email,
                            email_educacional,
                            ultimo_login, 
                            expiracao_recuperacao_senha, 
                            token_recuperacao_senha,
                            criado_em, 
                            criado_por, 
                            alterado_em, 
                            alterado_por, 
                            criado_login, 
                            alterado_login,
                            codigo_eol_unidade,
                            tipo,
                            possui_contrato_externo,
                            situacao_cadastro,
                            cpf,
                            tipo_email
                          from usuario 
                          where login = @login";

            return conexao.Obter().QueryFirstOrDefaultAsync<Usuario>(query, new { login });
        }
        public async Task<IEnumerable<Usuario>> ObterUsuarioInternoPorId(long[] ids)
        {
            var tipoUsuario = (int)TipoUsuario.Interno;
            var query = @"select 
                            id, 
                            login,                             
                            nome,       
                            email,
                            ultimo_login, 
                            expiracao_recuperacao_senha, 
                            token_recuperacao_senha,
                            criado_em, 
                            criado_por, 
                            alterado_em, 
                            alterado_por, 
                            criado_login, 
                            alterado_login,
                            codigo_eol_unidade,
                            tipo,
                            possui_contrato_externo,
                            situacao_cadastro,
                            cpf
                          from usuario 
                          where tipo = @tipoUsuario   AND id = any(@ids) ";

            return await conexao.Obter().QueryAsync<Usuario>(query, new { ids, tipoUsuario });
        }

        public Task<Usuario> ObterPorCpf(string cpf)
        {
            var query = @"select 
                            id, 
                            login,                             
                            nome,       
                            email,
                            ultimo_login, 
                            expiracao_recuperacao_senha, 
                            token_recuperacao_senha,
                            criado_em, 
                            criado_por, 
                            alterado_em, 
                            alterado_por, 
                            criado_login, 
                            alterado_login,
                            codigo_eol_unidade,
                            tipo,
                            possui_contrato_externo,
                            situacao_cadastro,
                            cpf
                          from usuario 
                          where cpf = @cpf";

            return conexao.Obter().QueryFirstOrDefaultAsync<Usuario>(query, new { cpf });
        }

        public Task AtivarCadastroUsuario(long usuarioId)
        {
            var situacaoCadastro = (int)SituacaoUsuario.Ativo;
            var query = @" UPDATE public.usuario
                            SET alterado_em= now(), alterado_por='Sistema',  alterado_login='Sistema', situacao_cadastro= @situacaoCadastro
                            WHERE id= @usuarioId ";

            return conexao.Obter().ExecuteAsync(query, new { usuarioId, situacaoCadastro });
        }
        public async Task<bool> AtualizarEmailEducacional(string login, string email)
        {
            var query = @" UPDATE public.usuario
                            SET alterado_em= now(), alterado_por='Sistema',  alterado_login='Sistema', email_educacional = @email
                            WHERE login= @login ";

            return await conexao.Obter().ExecuteAsync(query, new { login, email }) > 0;
        }

        public Task<string?> ObterEmailEducacionalPorLogin(string login)
        {
            var query = @"select 
	                        email_educacional as EmailEducacional
                        from usuario where login = @login";

            return conexao.Obter().QueryFirstOrDefaultAsync<string?>(query, new { login });
        }

        #region Usuario Rede Parceria

        public Task<int> ObterTotalUsuarioRedeParceria(long[] areaPromotoraIds, string? nome, string? cpf, SituacaoUsuario? situacao)
        {
            var tipo = TipoUsuario.RedeParceria;

            var query = new StringBuilder();
            query.Append(" SELECT COUNT(1) FROM usuario WHERE NOT excluido AND tipo = @tipo");

            if (areaPromotoraIds.PossuiElementos())
            {
                query.Append(" AND area_promotora_id = any(@areaPromotoraIds)");
            }

            if (nome.EstaPreenchido())
            {
                nome = "%" + nome.ToLower() + "%";
                query.Append(" AND lower(nome) LIKE @nome ");
            }

            if (cpf.PossuiElementos())
            {
                cpf = cpf.SomenteNumeros();
                query.Append(" AND cpf = @cpf");
            }

            if (situacao.NaoEhNulo())
            {
                query.Append(" AND situacao_cadastro = @situacao");
            }

            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { tipo, areaPromotoraIds, nome, cpf, situacao });
        }

        public Task<IEnumerable<Usuario>> ObterUsuarioRedeParceria(long[] areaPromotoraIds, string? nome, string? cpf, SituacaoUsuario? situacao, int numeroPagina, int numeroRegistros)
        {
            var tipo = TipoUsuario.RedeParceria;
            var registrosIgnorados = numeroPagina > 1 ? (numeroPagina - 1) * numeroRegistros : 0;
            
            var query = new StringBuilder();
            query.Append(" SELECT u.id, u.nome, u.cpf, u.email, u.telefone, u.situacao_cadastro, u.area_promotora_id, a.nome ");
            query.Append(" FROM usuario u ");
            query.Append(" LEFT JOIN area_promotora a ON a.id = u.area_promotora_id and not a.excluido ");
            query.Append(" WHERE not u.excluido AND u.tipo = @tipo ");

            if (areaPromotoraIds.PossuiElementos())
            {
                query.Append(" AND u.area_promotora_id = any(@areaPromotoraIds)");
            }

            if (nome.EstaPreenchido())
            {
                nome = "%" + nome.ToLower() + "%";
                query.Append(" AND lower(u.nome) LIKE @nome ");
            }

            if (cpf.PossuiElementos())
            {
                cpf = cpf.SomenteNumeros();
                query.Append(" AND u.cpf = @cpf");
            }

            if (situacao.NaoEhNulo())
            {
                query.Append(" AND u.situacao_cadastro = @situacao");
            }

            query.Append(" order by a.nome desc");
            query.Append(" limit @numeroRegistros offset @registrosIgnorados");

            return conexao.Obter().QueryAsync<Usuario, AreaPromotora, Usuario>(
                query.ToString(),
                (usuario, areaPromotora) =>
                {
                    usuario.AreaPromotora = areaPromotora;
                    return usuario;
                },
                new { tipo, areaPromotoraIds, nome, cpf, situacao, registrosIgnorados, numeroRegistros },
                splitOn: "id, area_promotora_id");
        }

        #endregion
    }
}
