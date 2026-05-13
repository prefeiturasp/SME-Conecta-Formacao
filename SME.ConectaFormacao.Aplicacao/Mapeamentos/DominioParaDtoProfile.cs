using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AnoTurma;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaCriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class DominioParaDtoProfile : Profile
    {
        private const string FormatoData = "dd/MM/yyyy";
        public DominioParaDtoProfile()
        {
            MapAuditoria();
            MapAreaPromotora();
            MapDre();
            MapProposta();
            MapPropostaParceista();
            MapPropostaDiversas();
            MapPropostaTurma();
            MapPropostaParaDTO();
            MapArquivo();
            MapInscricao();
            MapUsuarios();
            MapNotificacao();
            MapOutros();
        }

        private void MapAuditoria()
        {
            CreateMap<EntidadeBaseAuditavel, AuditoriaDTO>();
        }

        private void MapAreaPromotora()
        {
            CreateMap<AreaPromotora, AreaPromotoraPaginadaDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(x => x.Tipo.Nome()))
                .ForMember(dest => dest.NomeDre, opt => opt.MapFrom(x => x.Dre!.Nome))
                .ForMember(dest => dest.NomeCoordenadoria, opt => opt.MapFrom(x => FormatarNomeCoordenadoria(x.Coordenadoria)));

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

            CreateMap<AreaPromotora, PropostaInformacoesCadastranteDTO>()
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.Nome))
                .ForMember(dest => dest.AreaPromotoraEmails, opt => opt.MapFrom(x => x.Email.Replace(";", ", ")))
                .ForMember(dest => dest.AreaPromotoraTelefones, opt => opt.MapFrom(x => string.Join(", ", x.Telefones.Select(t => t.Telefone.Length > 10 ? t.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : t.Telefone.AplicarMascara(@"\(00\) 0000\-0000")))))
                .ForMember(dest => dest.AreaPromotoraTipo, opt => opt.MapFrom(x => x.Tipo.Nome()))
                .ForMember(dest => dest.AreaPromotoraTipoId, opt => opt.MapFrom(x => x.Tipo));

            CreateMap<AreaPromotora, PropostaAreaPromotoraDTO>();
        }

        private void MapDre()
        {
            CreateMap<PropostaDre, PropostaDreDTO>().ReverseMap();

            CreateMap<PropostaTurmaDre, PropostaTurmaDreDTO>()
                .ForMember(dest => dest.DreNome, opt => opt.MapFrom(o => o.Dre.Nome))
                .ReverseMap();

            CreateMap<PropostaTurmaDre, PropostaTurmaDreCompletoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Dre.Nome))
                .ReverseMap();

            CreateMap<Dre, DreDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));

            CreateMap<Dre, DreServicoEol>().ReverseMap();
        }


        private void MapProposta()
        {

            CreateMap<Proposta, RetornoListagemFormacaoDTO>()
                .ForMember(dest => dest.Titulo, opt => opt.MapFrom(o => o.NomeFormacao))
                .ForMember(dest => dest.AreaPromotora,
                    opt => opt.MapFrom(o => o.AreaPromotora != null
                        ? o.AreaPromotora.Nome
                        : null))
                .ForMember(dest => dest.TipoFormacaoDescricao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.FormatoDescricao, opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.InscricaoEncerrada, opt => opt.MapFrom(o => DateTimeExtension.HorarioBrasilia().Date > o.DataInscricaoFim))
                .ForMember(dest => dest.Periodo, opt => opt.MapFrom(o => $"{o.DataRealizacaoInicio.GetValueOrDefault():dd/MM/yyyy} até {o.DataRealizacaoFim.GetValueOrDefault():dd/MM/yyyy}"))
                .ForMember(dest => dest.PeriodoInscricao, opt => opt.MapFrom(o => $"{o.DataInscricaoInicio.GetValueOrDefault():dd/MM/yyyy} até {o.DataInscricaoFim.GetValueOrDefault():dd/MM/yyyy}"));

            CreateMap<Proposta, DadosListagemFormacaoComTurmaDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(o => o.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(o => o.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(o => o.Id));
        }

        private void MapPropostaParaDTO()
        {
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

            CreateMap<Proposta, PropostaPaginadaDTO>()
                .ForMember(dest => dest.TipoFormacao, opt => opt.MapFrom(x => x.TipoFormacao.HasValue ? x.TipoFormacao.Nome() : null))
                .ForMember(dest => dest.Formato, opt => opt.MapFrom(x => x.Formato.HasValue ? x.Formato.Nome() : null))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(x => x.Situacao.Nome()))
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(x => x.AreaPromotora.Nome))
                .ForMember(dest => dest.DataRealizacaoInicio, opt => opt.MapFrom(x => x.DataRealizacaoInicio.HasValue ? x.DataRealizacaoInicio.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(dest => dest.DataRealizacaoFim, opt => opt.MapFrom(x => x.DataRealizacaoFim.HasValue ? x.DataRealizacaoFim.Value.ToString("dd/MM/yyyy") : string.Empty))
                .ForMember(dest => dest.Revalidacao,
                    opt => opt.MapFrom(x => MapRevalidacao(x.Revalidacao)));
        }

        private static string? FormatarNomeCoordenadoria(Coordenadoria? coordenadoria)
        {
            if (coordenadoria == null)
                return null;

            if (string.IsNullOrWhiteSpace(coordenadoria.Sigla))
                return coordenadoria.Nome;

            return $"{coordenadoria.Sigla} - {coordenadoria.Nome}";
        }

        private static string MapRevalidacao(bool? revalidacao)
        {
            return revalidacao switch
            {
                null => "-",
                true => "Sim",
                false => "Não"
            };
        }

        private void MapPropostaParceista()
        {
            CreateMap<PropostaParecerista, PropostaPareceristaDTO>().ReverseMap();

            CreateMap<PropostaParecerista, PropostaPareceristaSugestaoDTO>()
                .ForMember(dest => dest.Parecerista, opt => opt.MapFrom(o => o.NomeParecerista));

            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoCadastroDTO>().ReverseMap();


            CreateMap<PropostaParecerista, PropostaPareceristaDTO>();

            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoCompletoDTO>().ReverseMap();

            CreateMap<PropostaPareceristaConsideracao, AuditoriaDTO>().ReverseMap();

            CreateMap<PropostaPareceristaConsideracao, PropostaPareceristaConsideracaoDTO>()
                .ForMember(dest => dest.Auditoria, opt => opt.MapFrom(o => o))
                .ReverseMap();
        }

        private void MapPropostaTurma()
        {
            CreateMap<PropostaEncontroTurma, PropostaEncontroTurmaDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Turma.Nome))
                .ReverseMap();

            CreateMap<PropostaRegenteTurma, PropostaRegenteTurmaDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Turma.Nome))
                .ReverseMap();

            CreateMap<PropostaTutorTurma, PropostaTutorTurmaDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Turma.Nome))
                .ReverseMap();

            CreateMap<PropostaTurma, PropostaTurmaDTO>()
                .ReverseMap();

            CreateMap<PropostaTurma, PropostaTurmaCompletoDTO>()
                .ReverseMap();

            CreateMap<PropostaTurma, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(o => o.Nome));

            CreateMap<PropostaAnoTurma, PropostaAnoTurmaDTO>().ReverseMap();
        }

        private void MapPropostaDiversas()
        {
            CreateMap<PropostaEncontro, PropostaEncontroDto>()
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
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(o => string.IsNullOrWhiteSpace(o.NomeTutor) ? null : o.NomeTutor.Trim().ToUpper()))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => string.IsNullOrWhiteSpace(o.Cpf) ? null : o.Cpf.SomenteNumeros()));

            CreateMap<PropostaEncontroData, PropostaEncontroDataDto>().ReverseMap();
            CreateMap<PropostaTipoInscricao, PropostaTipoInscricaoDTO>().ReverseMap();
            CreateMap<PropostaMovimentacao, PropostaMovimentacaoDTO>().ReverseMap();
            CreateMap<PropostaCriterioCertificacao, PropostaCriterioCertificacaoDto>();
            CreateMap<PropostaCriterioValidacaoInscricao, PropostaCriterioValidacaoInscricaoDTO>().ReverseMap();
            CreateMap<PropostaFuncaoEspecifica, PropostaFuncaoEspecificaDTO>().ReverseMap();
            CreateMap<PropostaVagaRemanecente, PropostaVagaRemanecenteDTO>().ReverseMap();
            CreateMap<PropostaPublicoAlvo, PropostaPublicoAlvoDTO>().ReverseMap();
            CreateMap<PropostaPalavraChave, PropostaPalavraChaveDTO>().ReverseMap();
            CreateMap<PropostaModalidade, PropostaModalidadeDTO>().ReverseMap();
            CreateMap<PropostaComponenteCurricular, PropostaComponenteCurricularDTO>().ReverseMap();
            CreateMap<PropostaCriterioCertificacao, CriterioCertificacaoDTO>().ReverseMap();

        }

        private void MapArquivo()
        {
            CreateMap<Arquivo, ArquivoDTO>().ReverseMap();
            CreateMap<ImportacaoArquivoDTO, ImportacaoArquivo>().ReverseMap();
            CreateMap<ImportacaoArquivoRegistroDto, ImportacaoArquivoRegistro>().ReverseMap();
            CreateMap<Arquivo, PropostaImagemDivulgacaoDTO>()
                .ForMember(dest => dest.ArquivoId, opt => opt.MapFrom(x => x.Id));
        }

        private void MapInscricao()
        {
            CreateMap<Inscricao, InscricaoDto>()
                .ForMember(dest => dest.UsuarioAcessibilidade, opt => opt.MapFrom(o => o.UsuarioAcessibilidade))
                .ReverseMap();

            CreateMap<Inscricao, InscricaoManualDTO>().ReverseMap();

            CreateMap<Inscricao, InscricaoAutomaticaDTO>().ReverseMap();

            CreateMap<Inscricao, InscricaoPaginadaDTO>()
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.NomeFormacao))
                .ForMember(dest => dest.NomeTurma, opt => opt.MapFrom(o => o.PropostaTurma.Nome))
                .ForMember(dest => dest.Datas, opt => opt.MapFrom(o => $"{o.PropostaTurma.Proposta.DataRealizacaoInicio.Value:dd/MM/yyyy} até {o.PropostaTurma.Proposta.DataRealizacaoFim.Value:dd/MM/yyyy}"))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Situacao.Nome()))
                .ForMember(dest => dest.Origem, opt => opt.MapFrom(o => o.Origem.Nome()))
                .ForMember(dest => dest.IntegrarNoSga, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.IntegrarNoSGA))
                .ForMember(dest => dest.Iniciado, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.DataRealizacaoInicio.Value.Date <= DateTimeExtension.HorarioBrasilia().Date))
                .ForMember(dest => dest.PodeCancelar, opt => opt.MapFrom(o => o.Situacao != Dominio.Enumerados.SituacaoInscricao.Cancelada && o.Situacao != Dominio.Enumerados.SituacaoInscricao.Transferida))
                .ForMember(dest => dest.DataInscricao, opt => opt.MapFrom(o => o.CriadoEm.ToString("dd/MM/yyyy HH:mm")));

            CreateMap<Inscricao, DadosListagemInscricaoPermissaoDto>()
                .ForMember(dest => dest.PodeConfirmar, opt => opt.MapFrom(o => o.Situacao == SituacaoInscricao.AguardandoAnalise || o.Situacao == SituacaoInscricao.EmEspera))
                .ForMember(dest => dest.PodeColocarEmEspera, opt => opt.MapFrom(o => o.Situacao == SituacaoInscricao.AguardandoAnalise))
                .ForMember(dest => dest.PodeCancelar, opt => opt.MapFrom(o => o.Situacao != SituacaoInscricao.Cancelada && o.Situacao != SituacaoInscricao.Transferida))
                .ForMember(dest => dest.PodeReativar, opt => opt.MapFrom(o => o.Situacao == SituacaoInscricao.Cancelada));

            CreateMap<Inscricao, DadosListagemInscricaoDto>()
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
                .ForMember(dest => dest.Iniciado, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.DataRealizacaoInicio.Value.Date <= DateTimeExtension.HorarioBrasilia().Date))
                .ForMember(d => d.Permissao, opt => opt.MapFrom(s => s))
                .ForMember(dest => dest.DataInscricao, opt => opt.MapFrom(o => o.CriadoEm.ToString("dd/MM/yyyy HH:mm")));

        }

        private void MapUsuarios()
        {
            CreateMap<Usuario, InscricaoAutomaticaDTO>()
                .ForMember(dest => dest.UsuarioRf, opt => opt.MapFrom(o => o.Login))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.UsuarioCpf, opt => opt.MapFrom(o => o.Cpf))
                .ReverseMap()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(o => TipoUsuario.Interno))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => SituacaoUsuario.Ativo));


            CreateMap<UsuarioExternoDTO, Usuario>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome.Trim()))
                .ReverseMap();


            CreateMap<RetornoUsuarioCpfNomeDTO, Usuario>().ReverseMap();
            CreateMap<RetornoUsuarioCpfNomeDTO, CursistaResumidoServicoEol>().ReverseMap();

            CreateMap<DadosUsuarioDTO, Usuario>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome.Trim()));



            CreateMap<Usuario, UsuarioRedeParceriaPaginadoDTO>()
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(o => o.AreaPromotora.Nome))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Cpf))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(o => o.Email))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(o => o.Telefone.EstaPreenchido() ? o.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : string.Empty))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Situacao.Nome()));

            CreateMap<Usuario, UsuarioRedeParceriaDTO>()
                .ForMember(dest => dest.AreaPromotoraId, opt => opt.MapFrom(o => o.AreaPromotoraId))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Cpf))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(o => o.Email))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(o => o.Telefone));

            CreateMap<RetornoUsuarioLoginNomeDTO, NotificacaoUsuario>();

            CreateMap<Usuario, NotificacaoUsuario>()
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => NotificacaoUsuarioSituacao.NaoLida));

            CreateMap<Usuario, DadosLoginUsuarioDto>();
        }

        private void MapNotificacao()
        {

            CreateMap<Notificacao, NotificacaoDTO>()
                .ForMember(dest => dest.CategoriaDescricao, opt => opt.MapFrom(o => o.Categoria.Nome()))
                .ForMember(dest => dest.TipoDescricao, opt => opt.MapFrom(o => o.Tipo.Nome()));

            CreateMap<Notificacao, NotificacaoPaginadoDTO>()
                .ForMember(dest => dest.CategoriaDescricao, opt => opt.MapFrom(o => o.Categoria.Nome()))
                .ForMember(dest => dest.TipoDescricao, opt => opt.MapFrom(o => o.Tipo.Nome()))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Usuarios.FirstOrDefault().Situacao))
                .ForMember(dest => dest.SituacaoDescricao, opt => opt.MapFrom(o => o.Usuarios.FirstOrDefault().Situacao.Nome()));

            CreateMap<PropostaPareceristaResumidoDTO, NotificacaoUsuario>();

            CreateMap<Notificacao, NotificacaoSignalRDTO>()
                .ForMember(dest => dest.Usuarios, opt => opt.MapFrom(o => o.Usuarios.Any() ? o.Usuarios.Select(s => s.Login) : ArraySegment<string>.Empty));

            CreateMap<NotificacaoUsuario, EnviarEmailDto>()
                .ForMember(dest => dest.NomeDestinatario, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.EmailDestinatario, opt => opt.MapFrom(src => src.Email));
        }

        private void MapOutros()
        {

            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();

            CreateMap<PalavraChave, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));

            CreateMap<CriterioCertificacao, RetornoListagemDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Descricao));


            CreateMap<CargoFuncao, CargoFuncaoDto>();
            CreateMap<PalavraChave, PalavraChaveDTO>();
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
                        $"{o.DataRealizacaoInicio.GetValueOrDefault():dd/MM/yyyy} - {o.DataRealizacaoFim.GetValueOrDefault():dd/MM/yyyy}"))
                .ForMember(dest => dest.PeriodoInscricao,
                opt => opt.MapFrom(o =>
                        $"{o.DataInscricaoInicio.GetValueOrDefault():dd/MM/yyyy} - {o.DataInscricaoFim.GetValueOrDefault():dd/MM/yyyy}"));

            CreateMap<FormacaoTurma, RetornoTurmaDetalheDTO>()
                .ForMember(dest => dest.Horario,
                    opt => opt.MapFrom(o => $" De {o.HoraInicio} - {o.HoraFim}"))
                .ForMember(dest => dest.Periodos,
                    opt => opt.MapFrom(x => x.Periodos.Select(s => FormatarPeriodoData(s))))
                .ForMember(dest => dest.DatasEncontros,
                    opt => opt.MapFrom(x =>
                        x.Periodos.SelectMany(p =>
                            GerarDatasEncontros(
                                p.DataInicio,
                                p.DataFim ?? p.DataInicio,
                                x.HoraInicio,
                                x.HoraFim
                            )
                        ).ToList()
                    ))
                .ForMember(dest => dest.DataEncontrosNovo,
                    opt => opt.MapFrom(x => MapDataEncontrosNovo(x)));



        CreateMap<CursistaServicoEol, CursistaServicoEol>().ReverseMap();


            CreateMap<PropostaParecerista, PropostaPareceristaResumidoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.NomeParecerista))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(o => o.RegistroFuncional));

            CreateMap<PropostaParecerista, PropostaPareceristaResumidoDTO>()
                .ForMember(dest => dest.Login, opt => opt.MapFrom(o => o.RegistroFuncional))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.NomeParecerista));
        }
        private static string FormatarPeriodoData(FormacaoTurmaData s) =>
            s.DataFim.HasValue
                ? $"{s.DataInicio:dd/MM/yyyy} - {s.DataFim.Value:dd/MM/yyyy}"
                : $"{s.DataInicio:dd/MM/yyyy}";

        private static List<DataEncontroNovoDto>? MapDataEncontrosNovo(FormacaoTurma x)
        {
            if (x.DatasNovo == null) return null;
            return x.DatasNovo.Select(p => new DataEncontroNovoDto
            {
                DataInicial = p.DataInicio.ToString(FormatoData),
                DataFinal = p.DataFim.HasValue ? p.DataFim.Value.ToString(FormatoData) : null,
                HoraInicial = p.HoraInicio,
                HoraFinal = p.HoraFim,
                ModeloHorario = p.ModeloHorario
            }).ToList();
        }

        private static List<string> GerarDatasEncontros(DateTime inicio, DateTime fim, string horaInicio, string horaFim)
        {
            var lista = new List<string>();
            var dataAtual = inicio.Date;

            while (dataAtual <= fim.Date)
            {
                lista.Add($"{dataAtual:dd/MM/yyyy} {horaInicio} - {horaFim}");
                dataAtual = dataAtual.AddDays(1);
            }

            return lista;
        }
    }
}