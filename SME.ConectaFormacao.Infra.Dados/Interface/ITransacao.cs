using System.Data;

namespace SME.ConectaFormacao.Infra.Dados;

public interface ITransacao
{
    IDbTransaction Iniciar();
    IDbTransaction Iniciar(IsolationLevel il);
}
