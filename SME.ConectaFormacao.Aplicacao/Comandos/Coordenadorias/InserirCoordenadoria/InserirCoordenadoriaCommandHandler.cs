using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria
{
    public class InserirCoordenadoriaCommandHandler(IRepositorioCoordenadoria repositorio) :
        IRequestHandler<InserirCoordenadoriaCommand, Resultado<CoordenadoriaDto>>
    {
        public async Task<Resultado<CoordenadoriaDto>> Handle(InserirCoordenadoriaCommand request, CancellationToken cancellationToken)
        {
            var entidade = new Coordenadoria
            {
                Nome = request.Nome,
                Sigla = request.Sigla
            };

            entidade.Id = await repositorio.Inserir(entidade);
            var dto = new CoordenadoriaDto
            {
                Nome = entidade.Nome,
                Sigla = entidade.Sigla,
                Id = entidade.Id,                
            };
            return dto;
        }
    }
}