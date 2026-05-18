using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes.ObterCargosFuncoesPorUsuarioId;

public class ObterCargosFuncoesPorUsuarioIdQuery(long usuarioId)
    : IRequest<IEnumerable<DadosInscricaoCargoEol>>
{
    public long UsuarioId { get; } = usuarioId;
}
