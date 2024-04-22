using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

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
            var situacaoCadastro = (int)SituacaoCadastroUsuario.Ativo;
            var query = @" UPDATE public.usuario
                            SET alterado_em= now(), alterado_por='Sistema',  alterado_login='Sistema', situacao_cadastro= @situacaoCadastro
                            WHERE id= @usuarioId ";

            return conexao.Obter().ExecuteAsync(query, new { usuarioId, situacaoCadastro });
        }
    }
}
