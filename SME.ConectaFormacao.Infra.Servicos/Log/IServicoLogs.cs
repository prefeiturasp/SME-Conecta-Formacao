using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Servicos.Log
{
    public interface IServicoLogs
    {
        Task Enviar(string mensagem, LogContexto contexto = LogContexto.Geral, LogNivel nivel = LogNivel.Critico, string observacao = "", string rastreamento = "");
    }
}
