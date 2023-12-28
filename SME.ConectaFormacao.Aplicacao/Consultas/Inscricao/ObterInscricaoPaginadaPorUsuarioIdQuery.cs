using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPaginadaPorUsuarioIdQuery : IRequest<PaginacaoResultadoDTO<InscricaoPaginadaDTO>>
    {
        public ObterInscricaoPaginadaPorUsuarioIdQuery(long usuarioId, int numeroPagina, int numeroRegistros)
        {
            UsuarioId = usuarioId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long UsuarioId { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }

    public class ObterInscricaoPaginadaPorUsuarioIdQueryValidator : AbstractValidator<ObterInscricaoPaginadaPorUsuarioIdQuery>
    {
        public ObterInscricaoPaginadaPorUsuarioIdQueryValidator()
        {
            RuleFor(r => r.UsuarioId)
                .NotEmpty()
                .WithMessage("É necessário informar o id do usuário para obter as inscrições");
        }
    }
}
