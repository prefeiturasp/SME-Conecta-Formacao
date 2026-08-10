using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public partial class CodafSuplementarProfile
    {
        [ExcludeFromCodeCoverage]
        public class CodafCursoNaoHomologadoProfile : PerfilMapeamentoCodafBase
        {
            public CodafCursoNaoHomologadoProfile()
            {
                CreateMap<FiltroCodafCursoNaoHomologadoDto, FiltroListagemResultadoCodafCursoNaoHomologadoDto>()
                    .AplicarMapeamentoFiltroPaginacao();

                CreateMap<CodafAnexoSalvarDto, CodafCursoNaoHomologadoAnexo>()
                    .ForMember(dest => dest.Extensao, opt => opt.MapFrom(src => ObterExtensaoArquivo(src.NomeArquivo)));

                CreateMap<CodafCursoNaoHomologadoAnexo, CodafCursoNaoHomologadoAnexoDto>()
                    .ForMember(dest => dest.UrlDownload, opt => opt.Ignore());

                CreateMap<ListagemResultadoCodafCursoNaoHomologadoDto, CodafCursoNaoHomologadoResumoDto>();

                CreateMap<CodafCursoNaoHomologadoInscritoSalvarDto, CodafCursoNaoHomologadoInscricao>();

                CreateMap<CodafCursoNaoHomologadoInscricao, CodafCursoNaoHomologadoInscritoDto>()
                    .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Inscricao != null && src.Inscricao.Usuario != null ? src.Inscricao.Usuario.Nome : string.Empty))
                    .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => ResolverEFormatarDocumento(src.Inscricao)));

                CreateMap<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto, CodafCursoNaoHomologadoInscritoTurmaDto>()
                    .ForMember(destino => destino.Nome, opt => opt.MapFrom(origem => origem.NomeExibicao))
                    .ForMember(destino => destino.Documento, opt => opt.MapFrom(origem => ResolverEFormatarDocumento(origem.Login, origem.Cpf)));

            }
        }
    }
}