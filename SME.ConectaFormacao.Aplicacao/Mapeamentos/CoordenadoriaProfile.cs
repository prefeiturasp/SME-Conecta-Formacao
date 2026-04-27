using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CoordenadoriaProfile : Profile
    {
        public CoordenadoriaProfile()
        {
            CreateMap<Coordenadoria, CoordenadoriaDto>();
        }
    }
}