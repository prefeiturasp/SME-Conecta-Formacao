using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaCommand : IRequest<long>
    {
        public InserirPropostaCommand(long areaPromotoraId, PropostaDTO propostaDTO)
        {
            PropostaDTO = propostaDTO;
            AreaPromotoraId = areaPromotoraId;
        }

        public long AreaPromotoraId { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    public class InserirPropostaCommandValidator : AbstractValidator<InserirPropostaCommand>
    {
        public InserirPropostaCommandValidator()
        {
            RuleFor(f => f.AreaPromotoraId)
                .GreaterThan(0)
                .WithMessage("É nescessário informar o Id da área promotora para inserir a proposta");

            RuleFor(f => f.PropostaDTO.TipoFormacao)
                .NotNull()
                .WithMessage("É nescessário informar o tipo de formação para inserir a proposta");

            RuleFor(f => f.PropostaDTO.Modalidade)
                .NotNull()
                .WithMessage("É nescessário informar a modalidade para inserir a proposta");

            When(f => f.PropostaDTO.TipoFormacao == TipoFormacao.Curso, () =>
            {
                RuleFor(x => x.PropostaDTO.Modalidade).NotEqual(Modalidade.Hibrido).WithMessage("É permitido a modalidade Híbrido somente para o tipo de formação evento");
            });

            RuleFor(f => f.PropostaDTO.TipoInscricao)
                .NotNull()
                .WithMessage("É nescessário informar o tipo de inscrição para inserir a proposta");

            RuleFor(f => f.PropostaDTO.PublicosAlvo)
                .NotEmpty()
                .WithMessage("É nescessário informar o público alvo para inserir a proposta");

            RuleFor(f => f.PropostaDTO.CriteriosValidacaoInscricao)
                .NotEmpty()
                .WithMessage("É nescessário informar os critérios de validação das inscrições para inserir a proposta");

            RuleFor(f => f.PropostaDTO.QuantidadeTurmas)
                .NotEmpty()
                .WithMessage("É nescessário informar a quantidade de turmas para inserir a proposta");

            RuleFor(f => f.PropostaDTO.QuantidadeVagasTurma)
                .NotEmpty()
                .WithMessage("É nescessário informar a quantidade de vagas por turma para inserir a proposta");
            
            RuleFor(f => f.PropostaDTO.Justificativa)
                .NotEmpty()
                .WithMessage("É nescessário informar a justificativa para alterar a proposta");
            
            RuleFor(f => f.PropostaDTO.Objetivos)
                .NotEmpty()
                .WithMessage("É nescessário informar os objetivos para alterar a proposta");
            
            RuleFor(f => f.PropostaDTO.ConteudoProgramatico)
                .NotEmpty()
                .WithMessage("É nescessário informar o conteúdo programático para alterar a proposta");
            
            RuleFor(f => f.PropostaDTO.ProcedimentoMetadologico)
                .NotEmpty()
                .WithMessage("É nescessário informar os procedimentos metadológicos para alterar a proposta");
            
            RuleFor(f => f.PropostaDTO.Referencia)
                .NotEmpty()
                .WithMessage("É nescessário informar a referência para alterar a proposta");
        }
    }
}
