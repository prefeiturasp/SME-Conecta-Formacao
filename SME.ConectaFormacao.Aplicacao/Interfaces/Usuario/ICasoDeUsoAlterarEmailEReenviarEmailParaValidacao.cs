using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao
    {
        Task<bool> Executar(AlterarEmailUsuarioDto emailUsuarioDto);
    }
}