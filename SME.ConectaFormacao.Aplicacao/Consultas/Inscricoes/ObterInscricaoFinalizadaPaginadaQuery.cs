using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoFinalizadaPaginadaQuery : IRequest<PaginacaoResultadoDto<InscricaoPaginadaDTO>>
    {
        public ObterInscricaoFinalizadaPaginadaQuery(long usuarioId, int numeroPagina, int numeroRegistros, InscricaoFinalizadaFiltro filtro)
        {
            UsuarioId = usuarioId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            Filtro = filtro;
        }

        public long UsuarioId { get; }
        public int NumeroPagina { get; }
        public int NumeroRegistros { get; }
        public InscricaoFinalizadaFiltro Filtro { get; }
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
