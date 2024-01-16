using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPorTipoInscricaoESituacaoQuery : IRequest<IEnumerable<FormacaoResumida>>
    {
        public ObterPropostaPorTipoInscricaoESituacaoQuery(TipoInscricao[] tiposInscricoes, SituacaoProposta situacao)
        {
            TiposInscricoes = tiposInscricoes;
            Situacao = situacao;
        }
        public TipoInscricao[] TiposInscricoes { get; set; }
        public SituacaoProposta Situacao { get; set; }
    }
    public class ObterPropostaPorTipoInscricaoESituacaoQueryValidator : AbstractValidator<ObterPropostaPorTipoInscricaoESituacaoQuery>
    {
        public ObterPropostaPorTipoInscricaoESituacaoQueryValidator()
        {
            RuleFor(x => x.TiposInscricoes)
                .NotEmpty()
                .WithMessage("É necessário informar o tipo da inscrição para obter a proposta");
            
            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("É necessário informar a situação para obter a proposta");
        }
    }
}