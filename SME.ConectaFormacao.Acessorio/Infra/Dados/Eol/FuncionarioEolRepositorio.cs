using Dapper;
using System.Data;

namespace SME.ConectaFormacao.Acessorio.Infra.Dados.Eol;

public class FuncionarioEolRepositorio
{
    private readonly IDbConnection conexao;

    public FuncionarioEolRepositorio(IDbConnection conexao)
    {
        this.conexao = conexao ?? throw new ArgumentNullException(nameof(conexao));
    }

    public Task<IEnumerable<string>> ObterFuncionariosPorTipoEscola(params int[] tipoEscola)
    {
        var query = @"
				select
					distinct servidor.cd_registro_funcional
				from v_servidor_cotic servidor with (nolock)
				inner join v_cargo_base_cotic cargobase with (nolock) on servidor.cd_servidor = cargobase.cd_servidor
				inner join lotacao_servidor ls with (nolock) on cargobase.cd_cargo_base_servidor = ls.cd_cargo_base_servidor
					and (ls.dt_fim is null 
						or(ls.dt_fim is not null
							and cast(getdate() as date) <= ls.dt_fim))
					and ls.dt_cancelamento is null
				inner join v_cadastro_unidade_educacao ue with(nolock) on ue.cd_unidade_educacao = ls.cd_unidade_educacao
				inner join escola esc on esc.cd_escola = ls.cd_unidade_educacao
				where
					esc.tp_escola in @tipoEscola and 
					cargobase.dt_cancelamento IS NULL
					AND cargobase.dt_fim_nomeacao IS NULL
				";

        return conexao.QueryAsync<string>(query, new { tipoEscola });
    }
}
