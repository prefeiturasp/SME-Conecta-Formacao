using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AnoTurma;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class ComumProfile : Profile
    {
        public ComumProfile()
        {
            CreateMap<EntidadeBaseAuditavel, AuditoriaDTO>();

            CreateMap<Arquivo, ArquivoDTO>().ReverseMap();

            CreateMap<PalavraChave, PalavraChaveDTO>();
            CreateMap<PalavraChave, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));

            CreateMap<CriterioCertificacao, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Descricao));
            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();
            CreateMap<CargoFuncao, CargoFuncaoDto>();
            CreateMap<CriterioValidacaoInscricao, CriterioValidacaoInscricaoDTO>();


            CreateMap<AnoTurma, RetornoListagemTodosDTO>().ReverseMap();
            CreateMap<AnoTurma, AnoTurmaDTO>().ReverseMap();
            CreateMap<ComponenteCurricular, RetornoListagemTodosDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(o => o.Nome))
                .ReverseMap();
            CreateMap<ComponenteCurricular, ComponenteCurricularDTO>().ReverseMap();
            CreateMap<AnoTurma, ComponenteCurricularAnoTurmaServicoEol>()
                .ForMember(dest => dest.CodigoAnoTurma, opt => opt.MapFrom(o => o.CodigoEOL))
                .ForMember(dest => dest.DescricaoSerieEnsino, opt => opt.MapFrom(o => o.Descricao))
                .ReverseMap();
            CreateMap<ComponenteCurricular, ComponenteCurricularAnoTurmaServicoEol>()
                .ForMember(dest => dest.CodigoComponenteCurricular, opt => opt.MapFrom(o => o.CodigoEOL))
                .ForMember(dest => dest.DescricaoComponenteCurricular, opt => opt.MapFrom(o => o.Nome))
                .ReverseMap();
        }
    }
}