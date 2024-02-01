using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Ue
{
    public interface ICasoDeUsoObterUePorCodigo
    {
        Task<UeServicoEol> Executar(string ueCodigo);
    }
}