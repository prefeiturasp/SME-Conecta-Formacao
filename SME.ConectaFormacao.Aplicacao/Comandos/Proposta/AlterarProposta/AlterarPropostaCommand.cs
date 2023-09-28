using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaCommand : IRequest<long>
    {
        public AlterarPropostaCommand(long id, PropostaDTO propostaDTO)
        {
            Id = id;
            PropostaDTO = propostaDTO;
        }

        public long Id { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    public class AlterarPropostaCommandValidator : AbstractValidator<AlterarPropostaCommand>
    {
        public AlterarPropostaCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É nescessário informar o Id para alterar a proposta");

            RuleFor(f => f.PropostaDTO.TipoFormacao)
                .NotNull()
                .WithMessage("É nescessário informar o tipo de formação para alterar a proposta");

            RuleFor(f => f.PropostaDTO.Modalidade)
                .NotNull()
                .WithMessage("É nescessário informar a modalidade para alterar a proposta");

            When(f => f.PropostaDTO.TipoFormacao == TipoFormacao.Curso, () =>
            {
                RuleFor(x => x.PropostaDTO.Modalidade).NotEqual(Modalidade.Hibrido).WithMessage("É permitido a modalidade Híbrido somente para o tipo de formação evento");
            });

            RuleFor(f => f.PropostaDTO.TipoInscricao)
                .NotNull()
                .WithMessage("É nescessário informar o tipo de inscrição para alterar a proposta");

            RuleFor(f => f.PropostaDTO.PublicosAlvo)
                .NotEmpty()
                .WithMessage("É nescessário informar o público alvo para alterar a proposta");

            RuleFor(f => f.PropostaDTO.CriteriosValidacaoInscricao)
                .NotEmpty()
                .WithMessage("É nescessário informar os critérios de validação das inscrições para alterar a proposta");

            RuleFor(f => f.PropostaDTO.QuantidadeTurmas)
                .NotEmpty()
                .WithMessage("É nescessário informar a quantidade de turmas para alterar a proposta");

            RuleFor(f => f.PropostaDTO.QuantidadeVagasTurma)
                .NotEmpty()
                .WithMessage("É nescessário informar a quantidade de vagas por turma para alterar a proposta");
        }
    }
}
