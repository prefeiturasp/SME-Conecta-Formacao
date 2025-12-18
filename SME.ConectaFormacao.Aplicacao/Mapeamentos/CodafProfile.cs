using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class CodafProfile : Profile
    {
        public CodafProfile()
        {
            CreateMap<CodafListaPresenca, CodafListaPresencaDto>();
        }
    }
}