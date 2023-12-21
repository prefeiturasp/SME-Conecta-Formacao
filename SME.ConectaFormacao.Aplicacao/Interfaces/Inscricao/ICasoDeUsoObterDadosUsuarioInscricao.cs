using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterDadosUsuarioInscricao
    {
        Task<DadosUsuarioInscricaoDTO> Executar();
    }
}
