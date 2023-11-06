﻿using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaCriterioCertificacao;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class DominioParaDTOProfile : Profile
    {
        public DominioParaDTOProfile()
        {
            CreateMap<EntidadeBaseAuditavel, AuditoriaDTO>();

            // -> Area Promotora
            CreateMap<AreaPromotora, AreaPromotoraPaginadaDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(x => x.Tipo.Nome()))
                .ForMember(dest => dest.NomeDre, opt => opt.MapFrom(x => x.Dre.Nome));

            CreateMap<AreaPromotora, AreaPromotoraCompletoDTO>()
                .ForMember(dst => dst.Emails, map => map.MapFrom(src => src.Email.Split(';', StringSplitOptions.None).Select(t => new AreaPromotoraEmailDTO { Email = t })));

            CreateMap<AreaPromotora, AreaPromotoraDTO>()
                .ForMember(dst => dst.Emails, map => map.MapFrom(src => src.Email.Split(';', StringSplitOptions.None).Select(t => new AreaPromotoraEmailDTO { Email = t })))
                .ReverseMap()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => string.Join(";", x.Emails.Select(t => t.Email))));

            CreateMap<AreaPromotoraTelefone, AreaPromotoraTelefoneDTO>()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.Length > 10 ? x.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : x.Telefone.AplicarMascara(@"\(00\) 0000\-0000")))
                .ReverseMap()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.SomenteNumeros()));

            CreateMap<AreaPromotora, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));
            
            CreateMap<PalavraChave, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));
            
            CreateMap<Dre, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));
            
            CreateMap<CriterioCertificacao, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Descricao));

            // -> Proposta
            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();
            CreateMap<CargoFuncao, CargoFuncaoDTO>();
            CreateMap<PalavraChave, PalavraChaveDTO>();
            CreateMap<PropostaCriterioCertificacao, PropostaCriterioCertificacaoDto>();
            CreateMap<CriterioValidacaoInscricao, CriterioValidacaoInscricaoDTO>();
            CreateMap<Proposta, PropostaCompletoDTO>();
            CreateMap<Proposta, PropostaDTO>()
                .ForMember(dest => dest.PublicosAlvo, opt => opt.MapFrom(o => o.PublicosAlvo))
                .ForMember(dest => dest.FuncoesEspecificas, opt => opt.MapFrom(o => o.FuncoesEspecificas))
                .ForMember(dest => dest.VagasRemanecentes, opt => opt.MapFrom(o => o.VagasRemanecentes))
                .ForMember(dest => dest.CriteriosValidacaoInscricao, opt => opt.MapFrom(o => o.CriteriosValidacaoInscricao))
                .ForMember(dest => dest.PalavrasChaves, opt => opt.MapFrom(o => o.PalavrasChaves))
                .ForMember(dest => dest.CriterioCertificacao, opt => opt.MapFrom(o => o.CriterioCertificacao))
                .ReverseMap();
            CreateMap<PropostaCriterioValidacaoInscricao, PropostaCriterioValidacaoInscricaoDTO>().ReverseMap();
            CreateMap<PropostaFuncaoEspecifica, PropostaFuncaoEspecificaDTO>().ReverseMap();
            CreateMap<PropostaVagaRemanecente, PropostaVagaRemanecenteDTO>().ReverseMap();
            CreateMap<PropostaPublicoAlvo, PropostaPublicoAlvoDTO>().ReverseMap();
            CreateMap<PropostaPalavraChave, PropostaPalavraChaveDTO>().ReverseMap();
            CreateMap<PropostaCriterioCertificacao, CriterioCertificacaoDTO>().ReverseMap();
            CreateMap<Proposta, PropostaPaginadaDTO>()
                .ForMember(dest => dest.TipoFormacao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.Modalidade, opt => opt.MapFrom(x => x.Modalidade.HasValue ? x.Modalidade.Nome() : null))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(x => x.Situacao.Nome()))
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.AreaPromotora.Nome));

            CreateMap<Arquivo, PropostaImagemDivulgacaoDTO>()
                .ForMember(dest => dest.ArquivoId, opt => opt.MapFrom(x => x.Id));

            CreateMap<PropostaEncontro, PropostaEncontroDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest => dest.Datas, opt => opt.MapFrom(o => o.Datas))
                .ReverseMap();
            
            CreateMap<PropostaRegente, PropostaRegenteDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest =>  dest.NomesTurmas,  opt => opt.MapFrom(o  =>  string.Join(", ",o.Turmas.Select(x => "Turma " + x.Turma)) ))
                .ReverseMap();
            
            CreateMap<PropostaTutor, PropostaTutorDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest =>  dest.NomesTurmas,  opt => opt.MapFrom(o  =>  string.Join(", ",o.Turmas.Select(x => "Turma " + x.Turma)) ))
                .ReverseMap();

            CreateMap<PropostaEncontroTurma, PropostaEncontroTurmaDTO>().ReverseMap();
            CreateMap<PropostaEncontroData, PropostaEncontroDataDTO>().ReverseMap();
            CreateMap<PropostaRegenteTurma, PropostaRegenteTurmaDTO>().ReverseMap();
            CreateMap<PropostaTutorTurma, PropostaTutorTurmaDTO>().ReverseMap();

            // -> Arquivo
            CreateMap<Arquivo, ArquivoDTO>().ReverseMap();
        }
    }
}
