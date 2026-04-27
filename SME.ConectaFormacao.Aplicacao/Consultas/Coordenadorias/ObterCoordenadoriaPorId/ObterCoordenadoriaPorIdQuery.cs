using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriaPorId
{
    public class ObterCoordenadoriaPorIdQuery(long Id) : IRequest<Resultado<CoordenadoriaDetalhadoDto>>
    {
        public long Id { get; set; } = Id;
    }
}
