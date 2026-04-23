using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriaPorId
{
    public class ObterCoordenadoriaPorIdQueryHandler(IRepositorioCoordenadoria repositorio) : 
        IRequestHandler<ObterCoordenadoriaPorIdQuery, Resultado<CoordenadoriaDetalhadoDto>>
    {
        public async Task<Resultado<CoordenadoriaDetalhadoDto>> Handle(ObterCoordenadoriaPorIdQuery request, CancellationToken cancellationToken)
        {
            var coordenadoria = await repositorio.ObterComAreaPromotoraAsync(request.Id);
            if (coordenadoria == null)
                return Erro.NaoEncontrado("Coordenadoria não encontrada.");
            var coordenadoriaDetalhadoDto = new CoordenadoriaDetalhadoDto
            {
                Id = coordenadoria.Id,
                Nome = coordenadoria.Nome,
                Sigla = coordenadoria.Sigla,
                AreasPromotoras = coordenadoria.AreasPromotoras.Select(a => new AreaPromotoraSimplificadoDto
                {
                    Id = a.Id,
                    Nome = a.Nome
                })
            };
            return coordenadoriaDetalhadoDto;
        }
    }
}
