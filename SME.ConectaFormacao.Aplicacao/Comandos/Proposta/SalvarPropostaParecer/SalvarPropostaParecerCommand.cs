using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaParecerCommand : IRequest<RetornoDTO>
    {
        public SalvarPropostaParecerCommand(PropostaParecerCadastroDTO propostaParecerCadastroDto, long usuarioLogadoId)
        {
            PropostaParecerCadastroDto = propostaParecerCadastroDto;
            UsuarioLogadoId = usuarioLogadoId;
        }

        public PropostaParecerCadastroDTO PropostaParecerCadastroDto { get; }
        public long UsuarioLogadoId { get; }
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
                .WithMessage("É necessário informar a descrição para salvar o parecer da proposta");
            
            RuleFor(x => x.UsuarioLogadoId)
                .NotEmpty()
                .WithMessage("É necessário informar o usuário logado para salvar o parecer da proposta");
        }
    }
}