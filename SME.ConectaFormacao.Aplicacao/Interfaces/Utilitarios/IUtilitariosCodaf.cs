using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios
{
    public interface IUtilitariosCodaf
    {
        static abstract TipoEstrategiaCodaf DefinirEstrategia(DadosProcessamentoCodafDto declaracao);

        Task EnviarEmailsAsync(List<EnviarEmailDto> notificacoesParaEnviar);

        Task SalvarLogAsync(string mensagem, LogNivel nivelLog = LogNivel.Informacao, Exception? ex = null);
    }
}
