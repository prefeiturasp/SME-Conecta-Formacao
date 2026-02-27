using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IUsuarioAcessibilidadeService
    {
        Task<long?> SalvarAcessibilidadeDaInscricaoAsync(UsuarioAcessibilidade usuarioAcessibilidade);
    }
}