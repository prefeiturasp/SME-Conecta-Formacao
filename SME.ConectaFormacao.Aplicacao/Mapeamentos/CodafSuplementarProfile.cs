using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafSuplementarProfile : Profile
    {
        public CodafSuplementarProfile()
        {
            CreateMap<CodafListaPresenca, CodafSuplementarDetalhadoDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(src => src.Proposta.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.Proposta.Id))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.Proposta.NumeroHomologacao))
                .ForMember(dest => dest.Retificacoes, opt => opt.MapFrom(src => src.CodafRetificacoes))
                .ForMember(dest => dest.Anexos, opt => opt.MapFrom(src => src.CodafAnexos))
                .ReverseMap()
                ;
            CreateMap<CodafSuplementar, CodafSuplementarDetalhadoDto>()
                .ForMember(dest => dest.CodafId, opt => opt.MapFrom(src => src.CodafId))
                .ForMember(dest => dest.PropostaId, opt => opt.MapFrom(src => src.Proposta.Id))
                .ForMember(dest => dest.PropostaTurmaId, opt => opt.MapFrom(src => src.PropostaTurma.Id))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(src => src.Proposta.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.Proposta.Id))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.Proposta.NumeroHomologacao))
                .ForMember(dest => dest.Retificacoes, opt => opt.MapFrom(src => src.CodafRetificacoes))
                .ForMember(dest => dest.Anexos, opt => opt.MapFrom(src => src.CodafAnexos))
                .ForMember(dest => dest.Inscritos, opt => opt.MapFrom(src => src.CodafInscricoes))
                .ForMember(dest => dest.Certificados, opt => opt.MapFrom(src => src.CodafCertificados))
                ;
            CreateMap<CodafSuplementarRetificacao, CodafSuplementarRetificacaoDto>().ReverseMap();
            CreateMap<CodafSuplementarRetificacaoSalvarDto, CodafSuplementarRetificacao>();
            CreateMap<CodafAnexoSalvarDto, CodafSuplementarAnexo>()
                .ForMember(dest => dest.Extensao, opt => opt.MapFrom(src => src.NomeArquivo.Substring(src.NomeArquivo.LastIndexOf('.') + 1)))
                ;
            CreateMap<CodafSuplementarAnexo, CodafSuplementarAnexoDto>().ForMember(dest => dest.UrlDownload, opt => opt.Ignore());
            CreateMap<CodafRetificacaoListaPresenca, CodafSuplementarRetificacaoDto>();
            CreateMap<CodafAnexo, CodafSuplementarAnexoDto>().ForMember(dest => dest.UrlDownload, opt => opt.Ignore());
            CreateMap<FiltroCodafSuplementarDto, FiltroListagemResultadoCodafSuplementarDto>()
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.CodigoFormacao.ToString()))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.NumeroHomologacao.ToString()))
                .ForMember(dest => dest.Pagina, opt => opt.MapFrom(src => src.NumeroPagina))
                .ForMember(dest => dest.TamanhoPagina, opt => opt.MapFrom(src => src.NumeroRegistros))
            ;
            CreateMap<ListagemResultadoCodafSuplementarDto, CodafSuplementarResumoDto>();

            CreateMap<CodafSuplementarInscritoSalvarDto, CodafSuplementarInscricao>();
            CreateMap<CodafSuplementarInscricao, CodafSuplementarInscritoDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Inscricao != null && src.Inscricao.Usuario != null ? src.Inscricao.Usuario.Nome : string.Empty))
                .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => ResolverEFormatarDocumento(src.Inscricao)))
                ;
            CreateMap<CodafCertificado, CodafCertificadoDto>();
        }
        private static string ResolverEFormatarDocumento(Inscricao? inscricao)
        {
            if (inscricao?.Usuario is null)
                return string.Empty;

            var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(inscricao.Usuario.Login, inscricao.Usuario.Cpf);
            return ResolvedorDocumentoUsuario.FormatarValor(documento, tipo);
        }
    }
}