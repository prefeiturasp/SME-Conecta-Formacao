using FluentValidation.Results;

namespace SME.ConectaFormacao.Dominio.Comum
{
    public static class ValidationExtensions
    {
        extension(ValidationResult result)
        {
            public Erro ToErroValidacao()
            {
                return Erro.Validacao(result.Errors.Select(e => e.ErrorMessage));
            }
        }
    }
}
