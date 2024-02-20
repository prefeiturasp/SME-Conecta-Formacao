using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Ue
{
    public interface ICasoDeUsoObterUnidadePorCodigoEol
    {
        Task<UnidadeEol> Executar(string codigoEolUnidade);
    }
}