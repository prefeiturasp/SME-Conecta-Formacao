using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class InscricaoProfile : Profile
    {
        public InscricaoProfile()
        {
            CreateMap<Inscricao, InscricaoDto>().ReverseMap();
            CreateMap<Inscricao, InscricaoManualDTO>().ReverseMap();
            CreateMap<Inscricao, InscricaoAutomaticaDTO>().ReverseMap();

            CreateMap<Inscricao, InscricaoPaginadaDTO>()
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.NomeFormacao))
                .ForMember(dest => dest.NomeTurma, opt => opt.MapFrom(o => o.PropostaTurma.Nome))
                .ForMember(dest => dest.Datas, opt => opt.MapFrom(o => $"{o.PropostaTurma.Proposta.DataRealizacaoInicio!.Value:dd/MM/yyyy} até {o.PropostaTurma.Proposta.DataRealizacaoFim!.Value:dd/MM/yyyy}"))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Situacao.Nome()))
                .ForMember(dest => dest.Origem, opt => opt.MapFrom(o => o.Origem.Nome()))
                .ForMember(dest => dest.IntegrarNoSga, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.IntegrarNoSGA))
                .ForMember(dest => dest.Iniciado, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.DataRealizacaoInicio!.Value.Date <= DateTimeExtension.HorarioBrasilia().Date))
                .ForMember(dest => dest.PodeCancelar, opt => opt.MapFrom(o => o.Situacao != SituacaoInscricao.Cancelada && o.Situacao != SituacaoInscricao.Transferida))
                .ForMember(dest => dest.DataInscricao, opt => opt.MapFrom(o => o.CriadoEm.ToString("dd/MM/yyyy HH:mm")));

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
                .ForMember(dest => dest.Iniciado, opt => opt.MapFrom(o => o.PropostaTurma.Proposta.DataRealizacaoInicio!.Value.Date <= DateTimeExtension.HorarioBrasilia().Date))
                .ForMember(d => d.Permissao, opt => opt.MapFrom(s => s))
                .ForMember(dest => dest.DataInscricao, opt => opt.MapFrom(o => o.CriadoEm.ToString("dd/MM/yyyy HH:mm")));
        }
    }
}