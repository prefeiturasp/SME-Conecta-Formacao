using Dapper;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioFuncaoAtividadeUsuario(IConectaFormacaoConexao conexao) : IRepositorioFuncaoAtividadeUsuario
    {
        public async Task<IEnumerable<FuncaoAtividadeServidorEol>> ObterPorRegistroFuncionalAsync(string cdRegistroFuncional)
        {
            const string query = @"SELECT id
                                        , cd_registro_funcional AS cdRegistroFuncional
                                        , cd_tipo_funcao AS cdTipoFuncao
                                        , codigo_ue AS cdUe
                                        , data_atualizacao AS dataAtualizacao
                                    FROM funcoes_atividades_eol
                                   WHERE cd_registro_funcional = @cdRegistroFuncional";

            var parametros = new DynamicParameters();
            parametros.Add("cdRegistroFuncional", cdRegistroFuncional);

            return await conexao.Obter().QueryAsync<FuncaoAtividadeServidorEol>(query, parametros);
        }

        public async Task<DateTime?> ObterDataUltimaAtualizacaoAsync()
        {
            const string query = @"SELECT MAX(data_atualizacao)
                                    FROM funcoes_atividades_eol";

            return await conexao.Obter().QueryFirstOrDefaultAsync<DateTime?>(query);
        }
    }
}
