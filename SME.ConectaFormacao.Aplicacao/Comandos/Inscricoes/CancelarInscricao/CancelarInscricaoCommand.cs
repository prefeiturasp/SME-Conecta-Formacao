using MediatR;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao
{
    public class CancelarInscricaoCommand(long id, string? motivo) : IRequest<bool>
    {
        public long Id { get; } = id;
        public string? Motivo { get; } = motivo;
    }
}
