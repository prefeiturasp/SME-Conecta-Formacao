using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo
{
    public interface ICasoDeUsoArquivoBaixar
    {
        Task<ArquivoBaixadoDTO> Executar(Guid codigoArquivo);
    }
}
