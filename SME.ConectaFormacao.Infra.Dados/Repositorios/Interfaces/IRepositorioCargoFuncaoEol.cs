using SME.ConectaFormacao.Infra.Dados.Dtos;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

public interface IRepositorioCargoFuncaoEol
{
    Task<IEnumerable<CargoFuncaoEolDto>> ObterCargosFuncoesEolDoServidorAsync(string rf);
}
