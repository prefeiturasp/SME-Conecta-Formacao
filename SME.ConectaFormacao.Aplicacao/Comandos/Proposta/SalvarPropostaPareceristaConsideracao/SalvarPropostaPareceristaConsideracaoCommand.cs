using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaConsideracaoCommand : IRequest<RetornoDTO>
    {
        public SalvarPropostaPareceristaConsideracaoCommand(PropostaPareceristaConsideracaoCadastroDTO propostaPareceristaConsideracaoCadastroDto, string login)
        {
            PropostaPareceristaConsideracaoCadastroDto = propostaPareceristaConsideracaoCadastroDto;
            Login = login;
        }

        public PropostaPareceristaConsideracaoCadastroDTO PropostaPareceristaConsideracaoCadastroDto { get; }
        public string Login { get; set; }
    }
    public class SalvarPropostaPareceristaConsideracaoCommandValidator : AbstractValidator<SalvarPropostaPareceristaConsideracaoCommand>
    {
        public SalvarPropostaPareceristaConsideracaoCommandValidator()
        {
            RuleFor(x => x.PropostaPareceristaConsideracaoCadastroDto.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id do parecerista da proposta para salvar a consideração do parecerista");
            
            RuleFor(x => x.PropostaPareceristaConsideracaoCadastroDto.Campo)
                .NotEmpty()
                .WithMessage("É necessário informar o campo para salvar a consideração do parecerista");

            RuleFor(x => x.PropostaPareceristaConsideracaoCadastroDto.Descricao)
                .NotEmpty()
                .WithMessage("É necessário informar a descrição para salvar a consideração do parecerista");
            
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login do usuário logado para salvar a consideração do parecerista");
        }
    }
}