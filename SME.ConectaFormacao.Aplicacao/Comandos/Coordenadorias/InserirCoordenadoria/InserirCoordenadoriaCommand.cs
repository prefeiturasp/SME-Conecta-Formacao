using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria
{
    public class InserirCoordenadoriaCommand(string Nome, string? Sigla) : IRequest<Resultado<CoordenadoriaDto>>
    {
        public string Nome { get; } = Nome;
        public string? Sigla { get; } = Sigla;
    }
}
