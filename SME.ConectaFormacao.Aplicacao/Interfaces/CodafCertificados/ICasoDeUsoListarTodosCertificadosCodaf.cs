using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados
{
    public interface ICasoDeUsoListarTodosCertificadosCodaf
    {
        Task<Resultado<PaginacaoResultadoDto<ListagemCertificadosCodafDto>>> ExecutarAsync(FiltroListaTodosCertificadosCodafDto filtro);

    }
}
