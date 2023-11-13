using FluentValidation.Results;

namespace SME.ConectaFormacao.Dominio.Excecoes
{
    public class ValidacaoException : Exception
    {
        public readonly IEnumerable<ValidationFailure> Erros;

        public ValidacaoException(IEnumerable<ValidationFailure> erros)
        {
            this.Erros = erros;
        }

        public List<string> Mensagens() => Erros?.Select(c => c.ErrorMessage)?.ToList();
    }
}
