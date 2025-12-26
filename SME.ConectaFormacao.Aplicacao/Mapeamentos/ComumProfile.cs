using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class ComumProfile : Profile
    {
        public ComumProfile()
        {
            CreateMap(typeof(ResultadoPaginado<>), typeof(PaginacaoResultadoDto<>))
                .ForMember("Items", opt => opt.MapFrom("Itens"))
                ;
        }
    }
}