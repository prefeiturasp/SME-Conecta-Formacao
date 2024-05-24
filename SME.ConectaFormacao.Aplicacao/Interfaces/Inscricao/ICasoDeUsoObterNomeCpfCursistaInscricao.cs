using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterNomeCpfCursistaInscricao
    {
        Task<RetornoUsuarioDTO> Executar(string? registroFuncional, string? cpf);
    }
}
