using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios
{
    public interface IUtilitariosCodaf
    {
        TipoEstrategiaCodaf DefinirEstrategia(DadosProcessamentoCodafDto declaracao);

        Task EnviarEmailsAsync(List<EnviarEmailDto> notificacoesParaEnviar);

        Task SalvarLogAsync(string mensagem, LogNivel nivelLog = LogNivel.Informacao, Exception? ex = null);
    }
}
