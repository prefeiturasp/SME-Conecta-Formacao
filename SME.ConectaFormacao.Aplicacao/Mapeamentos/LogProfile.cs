using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<LogDTO, Log>();
        }
    }
}