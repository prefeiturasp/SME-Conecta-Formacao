using MediatR;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria
{
    public class AlterarCoordenadoriaCommand(long Id, string Nome, string? Sigla) : IRequest<Resultado>
    {
        public long Id { get; set; } = Id;
        public string Nome { get; set; } = Nome;
        public string? Sigla { get; set; } = Sigla;
    }
}
