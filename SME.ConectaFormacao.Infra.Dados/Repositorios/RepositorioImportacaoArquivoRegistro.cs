using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioImportacaoArquivoRegistro : RepositorioBaseAuditavel<ImportacaoArquivoRegistro>, IRepositorioImportacaoArquivoRegistro
    {
        public RepositorioImportacaoArquivoRegistro(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }
    }
}
