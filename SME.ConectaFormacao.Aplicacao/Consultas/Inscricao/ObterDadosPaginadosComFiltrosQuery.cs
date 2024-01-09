using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQuery :IRequest<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>>
    {
        public ObterDadosPaginadosComFiltrosQuery(long usuarioId, int numeroPagina, int numeroRegistros, long? codigoFormacao, string? nomeFormacao)
        {
            UsuarioId = usuarioId;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            CodigoFormacao = codigoFormacao;
            NomeFormacao = nomeFormacao;
        }

        public long UsuarioId { get; set;}
        public int NumeroPagina { get; set;}
        public int NumeroRegistros { get; set;}
        public long? CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
    }

    public class ObterDadosPaginadosComFiltrosQueryValidator : AbstractValidator<ObterDadosPaginadosComFiltrosQuery>
    {
        public ObterDadosPaginadosComFiltrosQueryValidator()
        {
            RuleFor(r => r.UsuarioId)
                .NotEmpty()
                .WithMessage("É necessário informar o id do usuário para obter as inscrições");
        }
    }
}