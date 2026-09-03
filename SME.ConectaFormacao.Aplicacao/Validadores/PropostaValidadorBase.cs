using FluentValidation;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Validadores
{
    public abstract class PropostaValidadorBase<T> : AbstractValidator<T> where T : class
    {
        protected void AdicionarValidacoesComuns(Func<T, PropostaDTO> selectorProposta)
        {
            RuleFor(f => selectorProposta(f).TipoFormacao)
                .NotNull()
                .WithMessage("É necessário informar o tipo de formação");

            RuleFor(f => selectorProposta(f).Formato)
                .NotNull()
                .WithMessage("É necessário informar o formato");

            When(f => selectorProposta(f).TipoFormacao == TipoFormacao.Curso, () =>
            {
                RuleFor(x => selectorProposta(x).Formato)
                    .NotEqual(Formato.Hibrido)
                    .WithMessage("É permitido o formato Híbrido somente para o tipo de formação evento");
            });

            When(f => selectorProposta(f).CursoComCertificado, () =>
            {
                RuleFor(f => selectorProposta(f).IdEmissor)
                    .NotNull()
                    .WithMessage("É necessário informar o id do emissor quando o curso for com certificado")
                    .GreaterThan(0)
                    .WithMessage("O id do emissor deve ser maior que zero");

                RuleFor(f => selectorProposta(f).TipoEmissor)
                    .NotNull()
                    .WithMessage("É necessário informar o tipo do emissor quando o curso for com certificado");
            });

            RuleFor(f => selectorProposta(f).TiposInscricao)
                .NotNull()
                .WithMessage("É necessário informar o tipo de inscrição");

            RuleFor(f => selectorProposta(f).Dres)
                .NotEmpty()
                .WithMessage("É necessário informar a dre");

            RuleFor(f => selectorProposta(f).CriteriosValidacaoInscricao)
                .NotEmpty()
                .WithMessage("É necessário informar os critérios de validação das inscrições");

            RuleFor(f => selectorProposta(f).QuantidadeTurmas)
                .NotEmpty()
                .WithMessage("É necessário informar a quantidade de turmas");

            RuleFor(f => selectorProposta(f).QuantidadeVagasTurma)
                .NotEmpty()
                .WithMessage("É necessário informar a quantidade de vagas por turma");

            RuleFor(f => selectorProposta(f).Turmas)
                .NotEmpty()
                .WithMessage("É necessário informar a turma");

            RuleFor(f => selectorProposta(f).Justificativa)
                .NotEmpty()
                .WithMessage("É necessário informar a justificativa");

            RuleFor(f => selectorProposta(f).Objetivos)
                .NotEmpty()
                .WithMessage("É necessário informar os objetivos");

            RuleFor(f => selectorProposta(f).SobreEsteCurso)
                .NotEmpty()
                .WithMessage("É necessário informar sobre este curso");

            RuleFor(f => selectorProposta(f).ConteudoProgramatico)
                .NotEmpty()
                .WithMessage("É necessário informar o conteúdo programático");

            RuleFor(f => selectorProposta(f).ProcedimentoMetadologico)
                .NotEmpty()
                .WithMessage("É necessário informar os procedimentos metadológicos");

            RuleFor(f => selectorProposta(f).Referencia)
                .NotEmpty()
                .WithMessage("É necessário informar a referência");

            RuleFor(f => selectorProposta(f).PalavrasChaves)
                .NotNull()
                .WithMessage("É necessário informar as palavras-chaves");

            RuleFor(f => selectorProposta(f).LinkParaInscricoesExterna)
                .NotNull()
                .When(y => selectorProposta(y).TiposInscricao.NaoEhNulo() && selectorProposta(y).TiposInscricao.Any(tipo => tipo.TipoInscricao == TipoInscricao.Externa))
                .WithMessage("É necessário informar o link para inscrições");
        }
    }
}