using MediatR;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.RemoverCoordenadoria
{
    public class RemoverCoordenadoriaCommand(long Id) : IRequest<Resultado>
    {
        public long Id { get; set; } = Id;
    }
}