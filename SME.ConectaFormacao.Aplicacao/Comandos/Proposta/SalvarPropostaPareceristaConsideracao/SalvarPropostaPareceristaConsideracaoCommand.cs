using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaConsideracaoCommand : IRequest<RetornoDTO>
    {
        public SalvarPropostaPareceristaConsideracaoCommand(PropostaPareceristaConsideracaoCadastroDTO propostaPareceristaConsideracaoCadastroDto)
        {
            PropostaPareceristaConsideracaoCadastroDto = propostaPareceristaConsideracaoCadastroDto;
        }

        public PropostaPareceristaConsideracaoCadastroDTO PropostaPareceristaConsideracaoCadastroDto { get; }
    }
    public class SalvarPropostaPareceristaConsideracaoCommandValidator : AbstractValidator<SalvarPropostaPareceristaConsideracaoCommand>
    {
        public SalvarPropostaPareceristaConsideracaoCommandValidator()
        {
            RuleFor(x => x.PropostaPareceristaConsideracaoCadastroDto.PropostaPareceristaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id do parecerista da proposta para salvar a consideração do parecerista");
            
            RuleFor(x => x.PropostaPareceristaConsideracaoCadastroDto.Campo)
                .NotEmpty()
                .WithMessage("É necessário informar o campo para salvar a consideração do parecerista");

            RuleFor(x => x.PropostaPareceristaConsideracaoCadastroDto.Descricao)
                .NotEmpty()
                .WithMessage("É necessário informar a descrição para salvar a consideração do parecerista");
        }
    }
}