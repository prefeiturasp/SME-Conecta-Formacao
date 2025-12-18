using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaCriterioCertificacao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class PropostaProfile : Profile
    {
        public PropostaProfile()
        {
            // Cabeçalho e informações básicas
            CreateMap<AreaPromotora, PropostaInformacoesCadastranteDTO>()
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.Nome))
                .ForMember(dest => dest.AreaPromotoraEmails, opt => opt.MapFrom(x => x.Email.Replace(";", ", ")))
                .ForMember(dest => dest.AreaPromotoraTelefones, opt => opt.MapFrom(x => string.Join(", ", x.Telefones.Select(t => t.Telefone.Length > 10 ? t.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : t.Telefone.AplicarMascara(@"\(00\) 0000\-0000")))))
                .ForMember(dest => dest.AreaPromotoraTipo, opt => opt.MapFrom(x => x.Tipo.Nome()))
                .ForMember(dest => dest.AreaPromotoraTipoId, opt => opt.MapFrom(x => x.Tipo));

            CreateMap<Proposta, PropostaCompletoDTO>()
                .ForMember(dest => dest.NomeSituacao, opt => opt.MapFrom(x => x.Situacao.Nome()));

            CreateMap<Proposta, PropostaDTO>()
                .ForMember(dest => dest.PublicosAlvo, opt => opt.MapFrom(o => o.PublicosAlvo))
                .ForMember(dest => dest.FuncoesEspecificas, opt => opt.MapFrom(o => o.FuncoesEspecificas))
                .ForMember(dest => dest.VagasRemanecentes, opt => opt.MapFrom(o => o.VagasRemanecentes))
                .ForMember(dest => dest.CriteriosValidacaoInscricao, opt => opt.MapFrom(o => o.CriteriosValidacaoInscricao))
                .ForMember(dest => dest.PalavrasChaves, opt => opt.MapFrom(o => o.PalavrasChaves))
                .ForMember(dest => dest.CriterioCertificacao, opt => opt.MapFrom(o => o.CriterioCertificacao))
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest => dest.Modalidades, opt => opt.MapFrom(o => o.Modalidades))
                .ForMember(dest => dest.AnosTurmas, opt => opt.MapFrom(o => o.AnosTurmas))
                .ForMember(dest => dest.ComponentesCurriculares, opt => opt.MapFrom(o => o.ComponentesCurriculares))
                .ForMember(dest => dest.Pareceristas, opt => opt.MapFrom(o => o.Pareceristas))
                .ReverseMap();

            CreateMap<PropostaRegente, PropostaRegenteDTO>()
                 .ForMember(dest => dest.NomesTurmas, opt => opt.MapFrom(o => string.Join(", ", o.Turmas.Select(x => x.Turma.Nome))))
                 .ReverseMap()
                 .ForMember(dest => dest.NomeRegente, opt => opt.MapFrom(o => !string.IsNullOrWhiteSpace(o.NomeRegente) ? o.NomeRegente.Trim().ToUpper() : null))
                 .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => !string.IsNullOrWhiteSpace(o.Cpf) ? o.Cpf.SomenteNumeros() : null));

            CreateMap<PropostaTutor, PropostaTutorDTO>()
                .ForMember(dest => dest.NomesTurmas, opt => opt.MapFrom(o => string.Join(", ", o.Turmas.Select(x => x.Turma.Nome))))
                .ReverseMap()
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(o => string.IsNullOrWhiteSpace(o.NomeTutor) ? null : o.NomeTutor.Trim().ToUpper()))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => string.IsNullOrWhiteSpace(o.Cpf) ? null : o.Cpf.SomenteNumeros()));

            CreateMap<PropostaTurma, PropostaTurmaDTO>().ReverseMap();

            CreateMap<PropostaEncontro, PropostaEncontroDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest => dest.Datas, opt => opt.MapFrom(o => o.Datas))
                .ReverseMap();

            CreateMap<PropostaEncontroTurma, PropostaEncontroTurmaDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Turma.Nome))
                .ReverseMap();
            CreateMap<PropostaEncontroData, PropostaEncontroDataDTO>().ReverseMap();

            CreateMap<PropostaRegenteTurma, PropostaRegenteTurmaDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Turma.Nome))
                .ReverseMap();

            CreateMap<PropostaTutorTurma, PropostaTutorTurmaDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Turma.Nome))
                .ReverseMap();

            CreateMap<PropostaDre, PropostaDreDTO>().ReverseMap();

            CreateMap<PropostaTipoInscricao, PropostaTipoInscricaoDTO>().ReverseMap();

            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoCadastroDTO>().ReverseMap();

            CreateMap<PropostaParecerista, PropostaPareceristaSugestaoDTO>()
                .ForMember(dest => dest.Parecerista, opt => opt.MapFrom(o => o.NomeParecerista));

            CreateMap<PropostaMovimentacao, PropostaMovimentacaoDTO>().ReverseMap();

            CreateMap<PropostaTurmaDre, PropostaTurmaDreDTO>()
                .ForMember(dest => dest.DreNome, opt => opt.MapFrom(o => o.Dre.Nome))
                .ReverseMap();

            CreateMap<PropostaTurma, PropostaTurmaCompletoDTO>()
                .ReverseMap();

            CreateMap<PropostaTurmaDre, PropostaTurmaDreCompletoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Dre.Nome))
                .ReverseMap();

            CreateMap<PropostaTurma, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(o => o.Nome));

            // Listagens
            CreateMap<Proposta, PropostaPaginadaDTO>()
                .ForMember(dest => dest.TipoFormacao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.Formato, opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(x => x.Situacao.Nome()))
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.AreaPromotora!.Nome))
                .ForMember(dest => dest.DataRealizacaoInicio, opt => opt.MapFrom(x => x.DataRealizacaoInicio.HasValue ? x.DataRealizacaoInicio.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(dest => dest.DataRealizacaoFim, opt => opt.MapFrom(x => x.DataRealizacaoFim.HasValue ? x.DataRealizacaoFim.Value.ToString("dd/MM/yyyy") : string.Empty));

            CreateMap<Proposta, RetornoListagemFormacaoDTO>()
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(o => o.NomeFormacao))
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(o => o.AreaPromotora!.Nome))
                .ForMember(dest => dest.TipoFormacaoDescricao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.FormatoDescricao, opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.InscricaoEncerrada, opt => opt.MapFrom(o => DateTimeExtension.HorarioBrasilia().Date > o.DataInscricaoFim))
                .ForMember(dest => dest.Periodo, opt => opt.MapFrom(o => $"{o.DataRealizacaoInicio.GetValueOrDefault():dd/MM} até {o.DataRealizacaoFim.GetValueOrDefault():dd/MM}"))
                .ForMember(dest => dest.PeriodoInscricao, opt => opt.MapFrom(o => $"{o.DataInscricaoInicio.GetValueOrDefault():dd/MM} até {o.DataInscricaoFim.GetValueOrDefault():dd/MM}"));

            CreateMap<PropostaParecerista, PropostaPareceristaDTO>();
            CreateMap<PropostaCriterioCertificacao, PropostaCriterioCertificacaoDto>();
            CreateMap<PropostaCriterioValidacaoInscricao, PropostaCriterioValidacaoInscricaoDTO>().ReverseMap();
            CreateMap<PropostaFuncaoEspecifica, PropostaFuncaoEspecificaDTO>().ReverseMap();
            CreateMap<PropostaVagaRemanecente, PropostaVagaRemanecenteDTO>().ReverseMap();
            CreateMap<PropostaPublicoAlvo, PropostaPublicoAlvoDTO>().ReverseMap();
            CreateMap<PropostaPalavraChave, PropostaPalavraChaveDTO>().ReverseMap();
            CreateMap<PropostaModalidade, PropostaModalidadeDTO>().ReverseMap();
            CreateMap<PropostaAnoTurma, PropostaAnoTurmaDTO>().ReverseMap();
            CreateMap<PropostaComponenteCurricular, PropostaComponenteCurricularDTO>().ReverseMap();
            CreateMap<PropostaCriterioCertificacao, CriterioCertificacaoDTO>().ReverseMap();
            CreateMap<Arquivo, PropostaImagemDivulgacaoDTO>()
                .ForMember(dest => dest.ArquivoId, opt => opt.MapFrom(x => x.Id));

            CreateMap<Proposta, DadosListagemFormacaoComTurmaDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(o => o.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(o => o.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(o => o.Id));


            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoCompletoDTO>().ReverseMap();
            CreateMap<PropostaPareceristaConsideracao, AuditoriaDTO>().ReverseMap();
            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoDTO>()
                .ForMember(dest => dest.Auditoria, opt => opt.MapFrom(o => o))
                .ReverseMap();

            CreateMap<PropostaParecerista, PropostaPareceristaResumidoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.NomeParecerista))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(o => o.RegistroFuncional));
        }
    }
}