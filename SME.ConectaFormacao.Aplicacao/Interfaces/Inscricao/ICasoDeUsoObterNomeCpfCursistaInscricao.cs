using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterNomeCpfCursistaInscricao
    {
        Task<RetornoUsuarioCpfNomeDTO> Executar(string? registroFuncional, string? cpf);
    }
}
