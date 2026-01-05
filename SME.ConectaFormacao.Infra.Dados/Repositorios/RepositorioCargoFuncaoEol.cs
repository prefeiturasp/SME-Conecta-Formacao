using Dapper;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCargoFuncaoEol(IConectaFormacaoConexao conexao) : IRepositorioCargoFuncaoEol
    {
        public async Task<IEnumerable<CargoFuncaoEolDto>> ObterCargosFuncoesEolDoServidorAsync(string rf)
        {
            const string query = @"
            SELECT 
                  C.CD_CARGO                  AS Codigo
                , CASE C.SOBREPOSTO 
                    WHEN true THEN 2 
                    ELSE 1 
                  END                         AS TipoCargoFuncao
                , C.DATA_POSSE::timestamp     AS DataPosse
                , C.NOME_CARGO                AS Nome
                , C.TIPO_VINCULO              AS TipoVinculo
                , C.CODIGO_DRE                AS CodigoDre
                , C.CODIGO_UE                 AS CodigoUe
                , CFDE_CARGO.CARGO_FUNCAO_ID  AS CargoFuncaoId
                -- Campos da Função (SplitOn começa aqui)
                , F.CD_TIPO_FUNCAO            AS CodigoFuncao
                , F.DATA_POSSE::timestamp     AS DataPosse
                , F.NOME_FUNCAO               AS Nome
                , F.TIPO_VINCULO              AS TipoVinculo
                , F.CODIGO_DRE                AS CodigoDre
                , F.CODIGO_UE                 AS CodigoUe
                , CFDE_FUNCAO.CARGO_FUNCAO_ID AS CargoFuncaoId
            FROM CARGOS_EOL C
            LEFT JOIN CARGO_FUNCAO_DEPARA_EOL CFDE_CARGO ON CFDE_CARGO.CODIGO_CARGO_EOL = C.CD_CARGO
            LEFT JOIN FUNCOES_ATIVIDADES_EOL F 
                ON C.CD_CARGO_BASE_COTIC = F.CD_CARGO_BASE_COTIC 
                AND C.CD_REGISTRO_FUNCIONAL = F.CD_REGISTRO_FUNCIONAL 
            LEFT JOIN CARGO_FUNCAO_DEPARA_EOL CFDE_FUNCAO ON CFDE_FUNCAO.CODIGO_FUNCAO_EOL  = F.CD_TIPO_FUNCAO 
            WHERE C.CD_REGISTRO_FUNCIONAL = @rf";

            var cargoDictionary = new Dictionary<int, CargoFuncaoEolDto>();
            await conexao.Obter().QueryAsync<CargoFuncaoEolDto, FuncaoDoCargoEolDto, CargoFuncaoEolDto>(
                query,
                (cargo, funcao) =>
                {
                    if (!cargoDictionary.TryGetValue(cargo.Codigo, out var cargoEntry))
                    {
                        cargoEntry = cargo with { Funcoes = [] };
                        cargoDictionary.Add(cargo.Codigo, cargoEntry);
                    }
                    if (funcao is not null && !string.IsNullOrEmpty(funcao.Nome))
                    {
                        cargoEntry.Funcoes.Add(funcao);
                    }
                    return cargoEntry;
                },
                new { rf },
                splitOn: "CodigoFuncao"
            );
            return cargoDictionary.Values;
        }
    }
}
