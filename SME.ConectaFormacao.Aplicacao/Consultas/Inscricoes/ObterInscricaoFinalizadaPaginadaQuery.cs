using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoFinalizadaPaginadaQuery : IRequest<PaginacaoResultadoDto<InscricaoPaginadaDTO>>
    {
        public ObterInscricaoFinalizadaPaginadaQuery(long usuarioId, int numeroPagina, int numeroRegistros)
        {
            UsuarioId = usuarioId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
        }

        public long UsuarioId { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
    }

    public class ObterInscricaoFinalizadaPaginadaQueryValidator : AbstractValidator<ObterInscricaoFinalizadaPaginadaQuery>
    {
        public ObterInscricaoFinalizadaPaginadaQueryValidator()
        {
            RuleFor(r => r.UsuarioId)
                .NotEmpty()
                .WithMessage("É necessário informar o id do usuário para obter as inscrições");
        }
    }
}
