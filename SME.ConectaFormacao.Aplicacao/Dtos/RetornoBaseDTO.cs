using FluentValidation.Results;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.DTOS;

public class RetornoBaseDTO
{
    public RetornoBaseDTO(IEnumerable<ValidationFailure> validationFailures)
    {
        if (validationFailures != null && validationFailures.Any())
            Mensagens = validationFailures.Select(c => c.ErrorMessage).ToList();
    }
    public RetornoBaseDTO()
    {
        Mensagens = new List<string>();
    }

    public RetornoBaseDTO(List<string> mensagens)
    {
        Mensagens = mensagens;
    }


    [Required]
    public List<string> Mensagens { get; set; }

    public bool ExistemErros => Mensagens?.Any() ?? false;
}