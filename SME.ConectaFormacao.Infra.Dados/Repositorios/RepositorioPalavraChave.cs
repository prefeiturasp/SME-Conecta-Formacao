﻿using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPalavraChave : RepositorioBaseAuditavel<PalavraChave>, IRepositorioPalavraChave
    {
        public RepositorioPalavraChave(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        { }

        public Task<IEnumerable<PalavraChave>> ObterLista()
        {
            var query = "select id, nome from palavra_chave where not excluido order by nome";
            return conexao.Obter().QueryAsync<PalavraChave>(query);
        }
    }
}
