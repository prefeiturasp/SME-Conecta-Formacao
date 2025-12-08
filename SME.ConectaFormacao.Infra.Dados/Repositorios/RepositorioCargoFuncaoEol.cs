using Dapper;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCargoFuncaoEol(IConectaFormacaoConexao conexao) : IRepositorioCargoFuncaoEol
    {
        public async Task<IEnumerable<CargoFuncaoEolDto>> ObterCargosFuncoesEolDoServidorAsync(string rf)
        {
            const string query = @" SELECT CD_CARGO Codigo
                                         , CASE SOBREPOSTO WHEN true THEN 2
                                           ELSE 1 END Tipo
                                         , DATA_POSSE
                                         , NOME_CARGO Nome
                                         , TIPO_VINCULO TipoVinculo
                                    FROM PUBLIC.CARGOS_EOL
                                    WHERE CD_REGISTRO_FUNCIONAL = @rf
                                    UNION ALL
                                    SELECT CD_TIPO_FUNCAO Codigo
                                         , 3 Tipo 
                                         , DATA_POSSE
                                         , NOME_FUNCAO Nome
                                         , TIPO_VINCULO TipoVinculo
                                    FROM PUBLIC.FUNCOES_ATIVIDADES_EOL
                                    WHERE CD_REGISTRO_FUNCIONAL = @rf";

            return await conexao.Obter().QueryAsync<CargoFuncaoEolDto>(query, new { rf });
        }
    }
}
