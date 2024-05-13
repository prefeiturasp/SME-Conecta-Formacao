using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AnoTurma;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaCriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Servicos.Eol;

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
                .ForMember(dest => dest.NomeDre, opt => opt.MapFrom(x => x.Dre!.Nome));

            CreateMap<AreaPromotora, AreaPromotoraCompletoDTO>()
                .ForMember(dest => dest.DreId, opt => opt.MapFrom(x => x.DreId))
                .ForMember(dest => dest.NomeDre, opt => opt.MapFrom(x => x.Dre!.Nome))
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

            CreateMap<Dre, DreDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));

            CreateMap<Dre, DreServicoEol>().ReverseMap();

            CreateMap<CriterioCertificacao, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Descricao));

            // -> Proposta
            CreateMap<AreaPromotora, PropostaInformacoesCadastranteDTO>()
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.Nome))
                .ForMember(dest => dest.AreaPromotoraEmails, opt => opt.MapFrom(x => x.Email.Replace(";", ", ")))
                .ForMember(dest => dest.AreaPromotoraTelefones, opt => opt.MapFrom(x => string.Join(", ", x.Telefones.Select(t => t.Telefone.Length > 10 ? t.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : t.Telefone.AplicarMascara(@"\(00\) 0000\-0000")))))
                .ForMember(dest => dest.AreaPromotoraTipo, opt => opt.MapFrom(x => x.Tipo.Nome()))
                .ForMember(dest => dest.AreaPromotoraTipoId, opt => opt.MapFrom(x => x.Tipo));

            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();
            CreateMap<CargoFuncao, CargoFuncaoDTO>();
            CreateMap<PalavraChave, PalavraChaveDTO>();
            CreateMap<PropostaParecerista, PropostaPareceristaDTO>();
            CreateMap<PropostaCriterioCertificacao, PropostaCriterioCertificacaoDto>();
            CreateMap<CriterioValidacaoInscricao, CriterioValidacaoInscricaoDTO>();
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
            CreateMap<PropostaCriterioValidacaoInscricao, PropostaCriterioValidacaoInscricaoDTO>().ReverseMap();
            CreateMap<PropostaFuncaoEspecifica, PropostaFuncaoEspecificaDTO>().ReverseMap();
            CreateMap<PropostaVagaRemanecente, PropostaVagaRemanecenteDTO>().ReverseMap();
            CreateMap<PropostaPublicoAlvo, PropostaPublicoAlvoDTO>().ReverseMap();
            CreateMap<PropostaPalavraChave, PropostaPalavraChaveDTO>().ReverseMap();
            CreateMap<PropostaParecerista, PropostaPareceristaDTO>().ReverseMap();
            CreateMap<PropostaModalidade, PropostaModalidadeDTO>().ReverseMap();
            CreateMap<PropostaAnoTurma, PropostaAnoTurmaDTO>().ReverseMap();
            CreateMap<PropostaComponenteCurricular, PropostaComponenteCurricularDTO>().ReverseMap();
            CreateMap<PropostaCriterioCertificacao, CriterioCertificacaoDTO>().ReverseMap();
            CreateMap<Proposta, PropostaPaginadaDTO>()
                .ForMember(dest => dest.TipoFormacao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.Formato, opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(x => x.Situacao.Nome()))
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.AreaPromotora.Nome))
                .ForMember(dest => dest.DataRealizacaoInicio, opt => opt.MapFrom(x => x.DataRealizacaoInicio.HasValue ? x.DataRealizacaoInicio.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(dest => dest.DataRealizacaoFim, opt => opt.MapFrom(x => x.DataRealizacaoFim.HasValue ? x.DataRealizacaoFim.Value.ToString("dd/MM/yyyy") : string.Empty));

            CreateMap<Arquivo, PropostaImagemDivulgacaoDTO>()
                .ForMember(dest => dest.ArquivoId, opt => opt.MapFrom(x => x.Id));

            CreateMap<PropostaEncontro, PropostaEncontroDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest => dest.Datas, opt => opt.MapFrom(o => o.Datas))
                .ReverseMap();

            CreateMap<PropostaRegente, PropostaRegenteDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest => dest.NomesTurmas, opt => opt.MapFrom(o => string.Join(", ", o.Turmas.Select(x => x.Turma.Nome))))
                .ReverseMap()
                .ForMember(dest => dest.NomeRegente, opt => opt.MapFrom(o => o.NomeRegente.NaoEhNulo() ? o.NomeRegente.Trim().ToUpper() : null))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Cpf.NaoEhNulo() ? o.Cpf.SomenteNumeros() : null));

            CreateMap<PropostaTutor, PropostaTutorDTO>()
                .ForMember(dest => dest.Turmas, opt => opt.MapFrom(o => o.Turmas))
                .ForMember(dest => dest.NomesTurmas, opt => opt.MapFrom(o => string.Join(", ", o.Turmas.Select(x => x.Turma.Nome))))
                .ReverseMap()
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(o => o.NomeTutor.NaoEhNulo() ? o.NomeTutor.Trim().ToUpper() : null))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Cpf.NaoEhNulo() ? o.Cpf.SomenteNumeros() : null));

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
                .ForMember(dest => dest.Parecerista, opt => opt.MapFrom(o => o.NomeParecerista))
                .ForMember(dest => dest.Sugestao, opt => opt.MapFrom(o => o.Situacao.Nome()));

            // -> Arquivo
            CreateMap<Arquivo, ArquivoDTO>().ReverseMap();

            CreateMap<PropostaMovimentacao, PropostaMovimentacaoDTO>().ReverseMap();

            CreateMap<AnoTurma, RetornoListagemTodosDTO>().ReverseMap();
            CreateMap<AnoTurma, AnoTurmaDTO>().ReverseMap();
            CreateMap<ComponenteCurricular, RetornoListagemTodosDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(o => o.Nome))
                .ReverseMap();

            CreateMap<PropostaTurma, PropostaTurmaDTO>()
                .ReverseMap();

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
            CreateMap<ComponenteCurricular, ComponenteCurricularDTO>().ReverseMap();
            CreateMap<AnoTurma, ComponenteCurricularAnoTurmaServicoEol>()
                .ForMember(dest => dest.CodigoAnoTurma, opt => opt.MapFrom(o => o.CodigoEOL))
                .ForMember(dest => dest.DescricaoSerieEnsino, opt => opt.MapFrom(o => o.Descricao))
                .ReverseMap();
            CreateMap<ComponenteCurricular, ComponenteCurricularAnoTurmaServicoEol>()
                .ForMember(dest => dest.CodigoComponenteCurricular, opt => opt.MapFrom(o => o.CodigoEOL))
                .ForMember(dest => dest.DescricaoComponenteCurricular, opt => opt.MapFrom(o => o.Nome))
                .ReverseMap();

            CreateMap<Proposta, RetornoListagemFormacaoDTO>()
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(o => o.NomeFormacao))
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(o => o.AreaPromotora.Nome))
                .ForMember(dest => dest.TipoFormacaoDescricao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.FormatoDescricao, opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.InscricaoEncerrada, opt => opt.MapFrom(o => DateTimeExtension.HorarioBrasilia().Date > o.DataInscricaoFim))
                .ForMember(dest => dest.Periodo, opt => opt.MapFrom(o => $"{o.DataRealizacaoInicio.GetValueOrDefault():dd/MM} até {o.DataRealizacaoFim.GetValueOrDefault():dd/MM}"))
                .ForMember(dest => dest.PeriodoInscricao, opt => opt.MapFrom(o => $"{o.DataInscricaoInicio.GetValueOrDefault():dd/MM} até {o.DataInscricaoFim.GetValueOrDefault():dd/MM}"));

            CreateMap<FormacaoDetalhada, RetornoFormacaoDetalhadaDTO>()
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(x => x.NomeFormacao))
                .ForMember(dest => dest.Justificativa, opt => opt.MapFrom(x => x.Justificativa.RemoverTagsHtml()))
                .ForMember(dest => dest.TipoFormacaoDescricao,
                    opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.FormatoDescricao,
                    opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.DataInscricaoFim, opt => opt.MapFrom(o => o.DataInscricaoFim))
                .ForMember(dest => dest.Periodo,
                    opt => opt.MapFrom(o =>
                        $"De {o.DataRealizacaoInicio.GetValueOrDefault():dd/MM} até {o.DataRealizacaoFim.GetValueOrDefault():dd/MM}"))
                .ForMember(dest => dest.PeriodoInscricao,
                opt => opt.MapFrom(o =>
                        $"De {o.DataInscricaoInicio.GetValueOrDefault():dd/MM} até {o.DataInscricaoFim.GetValueOrDefault():dd/MM}"));

            CreateMap<FormacaoTurma, RetornoTurmaDetalheDTO>()
                .ForMember(dest => dest.Horario,
                    opt => opt.MapFrom(o => $"{o.HoraInicio} até {o.HoraFim}"))
                .ForMember(dest => dest.Periodos,
                    opt =>
                        opt.MapFrom(x => x.Periodos.Select(s => s.DataFim.HasValue ? $"De {s.DataInicio:dd/MM} até {s.DataFim.Value:dd/MM}" : $"{s.DataInicio:dd/MM}")));

            CreateMap<Inscricao, InscricaoDTO>().ReverseMap();

            CreateMap<Inscricao, InscricaoAutomaticaDTO>().ReverseMap();

            CreateMap<CursistaServicoEol, CursistaServicoEol>().ReverseMap();

            CreateMap<Usuario, InscricaoAutomaticaDTO>()
                .ForMember(dest => dest.UsuarioRf, opt => opt.MapFrom(o => o.Login))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.UsuarioCpf, opt => opt.MapFrom(o => o.Cpf))
                .ReverseMap()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(o => TipoUsuario.Interno))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => SituacaoCadastroUsuario.Ativo));

            CreateMap<Inscricao, InscricaoPaginadaDTO>()
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.NomeFormacao))
                .ForMember(dest => dest.NomeTurma, opt => opt.MapFrom(o => o.PropostaTurma.Nome))
                .ForMember(dest => dest.Datas, opt => opt.MapFrom(o => $"{o.PropostaTurma.Proposta.DataRealizacaoInicio.Value:dd/MM/yyyy} até {o.PropostaTurma.Proposta.DataRealizacaoFim.Value:dd/MM/yyyy}"))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Situacao.Nome()))
                .ForMember(dest => dest.Origem, opt => opt.MapFrom(o => o.Origem.Nome()))
                .ForMember(dest => dest.IntegrarNoSga, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.IntegrarNoSGA))
                .ForMember(dest => dest.Iniciado, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.DataRealizacaoInicio.Value.Date <= DateTimeExtension.HorarioBrasilia().Date))
                .ForMember(dest => dest.PodeCancelar, opt => opt.MapFrom(o => o.Situacao != Dominio.Enumerados.SituacaoInscricao.Cancelada));

            CreateMap<Inscricao, DadosListagemInscricaoDTO>()
                .ForMember(dest => dest.NomeTurma, opt => opt.MapFrom(o => o.PropostaTurma.Nome))
                .ForMember(dest => dest.NomeCursista, opt => opt.MapFrom(o => o.Usuario.Nome))
                .ForMember(dest => dest.RegistroFuncional, opt => opt.MapFrom(o => o.Usuario.Login))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Usuario.Cpf))
                .ForMember(dest => dest.CargoFuncao, opt => opt.MapFrom(o => o.Funcao.Nome))
                .ForMember(dest => dest.SituacaoCodigo, opt => opt.MapFrom(o => o.Situacao))
                .ForMember(dest => dest.InscricaoId, opt => opt.MapFrom(o => o.Id))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Situacao.Nome()))
                .ForMember(dest => dest.Origem, opt => opt.MapFrom(o => o.Origem.Nome()))
                .ForMember(dest => dest.IntegrarNoSga, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.IntegrarNoSGA))
                .ForMember(dest => dest.Iniciado, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.DataRealizacaoInicio.Value.Date <= DateTimeExtension.HorarioBrasilia().Date));

            CreateMap<Proposta, DadosListagemFormacaoComTurmaDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(o => o.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(o => o.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(o => o.Id));

            CreateMap<UsuarioExternoDTO, Usuario>().ReverseMap();

            CreateMap<ImportacaoArquivoDTO, ImportacaoArquivo>().ReverseMap();
            CreateMap<ImportacaoArquivoRegistroDTO, ImportacaoArquivoRegistro>().ReverseMap();

            CreateMap<RetornoUsuarioCpfNomeDTO, Usuario>().ReverseMap();
            CreateMap<RetornoUsuarioCpfNomeDTO, CursistaResumidoServicoEol>().ReverseMap();

            CreateMap<DadosUsuarioDTO, Usuario>();

            CreateMap<AreaPromotora, PropostaAreaPromotoraDTO>();
            
            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoCompletoDTO>().ReverseMap();
            CreateMap<PropostaPareceristaConsideracao, AuditoriaDTO>().ReverseMap();
            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoDTO>()
                .ForMember(dest => dest.Auditoria, opt => opt.MapFrom(o => o))
                .ReverseMap();
        }
    }
}