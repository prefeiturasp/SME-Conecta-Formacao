using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<SalvarLogCommand, Log>();
        }
    }
}