using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaParecerCommand : IRequest<long>
    {
        public SalvarPropostaParecerCommand(PropostaParecerCadastroDTO propostaParecerCadastroDto)
        {
            PropostaParecerCadastroDto = propostaParecerCadastroDto;
        }

        public PropostaParecerCadastroDTO PropostaParecerCadastroDto { get; }
    }
    public class SalvarPropostaParecerCommandValidator : AbstractValidator<SalvarPropostaParecerCommand>
    {
        public SalvarPropostaParecerCommandValidator()
        {
            RuleFor(x => x.PropostaParecerCadastroDto.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar o parecer da proposta");
            
            RuleFor(x => x.PropostaParecerCadastroDto.Campo)
                .NotEmpty()
                .WithMessage("É necessário informar o campo para salvar o parecer da proposta");

            RuleFor(x => x.PropostaParecerCadastroDto.Descricao)
                .NotEmpty()
                .MinimumLength(1).WithMessage("É necessário informar a descrição para salvar o parecer da proposta");
        }
    }
}