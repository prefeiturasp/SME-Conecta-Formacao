using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class DominioParaDTOProfile : Profile
    {
        public DominioParaDTOProfile()
        {
            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();
            CreateMap<CargoFuncao, CargoFuncaoDTO>();
        }
    }
}
