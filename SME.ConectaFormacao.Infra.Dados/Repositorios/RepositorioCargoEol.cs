using Dapper;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCargoEol(IConectaFormacaoConexao conexao) : IRepositorioCargoEol
    {
        public async Task<IEnumerable<CargoEol>> ObterCargosEolPorDreAsync(string codigoDre)
        {
            const string query = @" SELECT id
                                         , cd_cargo AS cdCargo
                                         , CD_REGISTRO_FUNCIONAL AS cdRegistroFuncional
                                         , CODIGO_DRE AS codigoDre
                                         , CODIGO_UE AS codigoUe
                                         , SOBREPOSTO 
                                         , DATA_ATUALIZACAO AS dataAtualizacao
                                     FROM  cargos_eol
                                    WHERE  cargos_eol.CODIGO_DRE = @codigoDre";

            var parametros = new DynamicParameters();
            parametros.Add("codigoDre", codigoDre);

            return await conexao.Obter().QueryAsync<CargoEol>(query, parametros);
        }
    }
}