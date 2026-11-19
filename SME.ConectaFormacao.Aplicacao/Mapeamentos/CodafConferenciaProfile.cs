using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafConferenciaProfile : PerfilMapeamentoCodafBase
    {
        public CodafConferenciaProfile()
        {
            CreateMap<CodafConferencia, CodafConferenciaDetalhadoDto>()
                .ForMember(dest => dest.PropostaId, opt => opt.MapFrom(src => src.PropostaId))
                .ForMember(dest => dest.PropostaTurmaId, opt => opt.MapFrom(src => src.PropostaTurmaId))
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(src => src.Proposta != null ? src.Proposta.NomeFormacao : null))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.Proposta != null ? (long?)src.Proposta.Id : src.PropostaId))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.Proposta != null ? src.Proposta.NumeroHomologacao : null))
                .ForMember(dest => dest.CertificadoEmitido, opt => opt.MapFrom(src => src.CertificadoEmitido))
                .ForMember(dest => dest.Turma, opt => opt.MapFrom(src => src.PropostaTurma))
                .ForMember(dest => dest.Anexos, opt => opt.MapFrom(src => src.CodafAnexos))
                .ForMember(dest => dest.Inscritos, opt => opt.MapFrom(src => src.CodafInscricoes));

            CreateMap<FiltroCodafConferenciaDto, FiltroListagemResultadoCodafConferenciaDto>()
                .AplicarMapeamentoFiltroPaginacao();

            CreateMap<CodafAnexoSalvarDto, CodafConferenciaAnexo>()
                .ForMember(dest => dest.Extensao, opt => opt.MapFrom(src => ObterExtensaoArquivo(src.NomeArquivo)));

            CreateMap<CodafConferenciaAnexo, CodafConferenciaAnexoDto>()
                .ForMember(dest => dest.UrlDownload, opt => opt.Ignore());

            CreateMap<ListagemResultadoCodafConferenciaDto, CodafConferenciaResumoDto>();

            CreateMap<CodafConferenciaInscritoSalvarDto, CodafConferenciaInscricao>();

            CreateMap<CodafConferenciaInscricao, CodafConferenciaInscritoDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Inscricao != null && src.Inscricao.Usuario != null ? src.Inscricao.Usuario.Nome : string.Empty))
                .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => ResolverEFormatarDocumento(src.Inscricao)));

            CreateMap<ResultadoInscritoTurmaCodafConferenciaDto, CodafConferenciaInscritoTurmaDto>()
                .ForMember(destino => destino.Nome, opt => opt.MapFrom(origem => origem.NomeExibicao))
                .ForMember(destino => destino.Documento, opt => opt.MapFrom(origem => ResolverEFormatarDocumento(origem.Login, origem.Cpf)));
        }
    }
}
