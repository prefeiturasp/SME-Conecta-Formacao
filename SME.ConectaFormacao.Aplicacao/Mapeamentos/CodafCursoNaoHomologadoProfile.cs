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
                CreateMap<CodafCursoNaoHomologado, CodafCursoNaoHomologadoDetalhadoDto>()
                    .ForMember(dest => dest.PropostaId, opt => opt.MapFrom(src => src.PropostaId))
                    .ForMember(dest => dest.PropostaTurmaId, opt => opt.MapFrom(src => src.PropostaTurmaId))
                    .ForMember(dest => dest.Anexos, opt => opt.MapFrom(src => src.CodafAnexos))
                    .ForMember(dest => dest.Inscritos, opt => opt.MapFrom(src => src.CodafInscricoes));

                CreateMap<PropostaTurma, PropostaTurmaComCodafDto>()
                    .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Nome))
                    .ForMember(dest => dest.CodafId, opt => opt.MapFrom(src => src.CodafListaPresenca != null ? src.CodafListaPresenca.Id : 0));

                CreateMap<FiltroCodafCursoNaoHomologadoDto, FiltroListagemResultadoCodafCursoNaoHomologadoDto>()
                    .AplicarMapeamentoFiltroPaginacao();

                CreateMap<CodafAnexoSalvarDto, CodafCursoNaoHomologadoAnexo>()
                    .ForMember(dest => dest.Extensao, opt => opt.MapFrom(src => ObterExtensaoArquivo(src.NomeArquivo)));

                CreateMap<CodafCursoNaoHomologadoAnexo, CodafCursoNaoHomologadoAnexoDto>()
                    .ForMember(dest => dest.UrlDownload, opt => opt.Ignore());

                CreateMap<ListagemResultadoCodafCursoNaoHomologadoDto, CodafCursoNaoHomologadoResumoDto>();

                CreateMap<CodafCursoNaoHomologadoInscritoSalvarDto, CodafCursoNaoHomologadoInscricao>();

                CreateMap<CodafCursoNaoHomologadoInscricao, CodafCursoNaoHomologadoInscritoDto>();

                CreateMap<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto, CodafCursoNaoHomologadoInscritoTurmaDto>()
                    .ForMember(destino => destino.Nome, opt => opt.MapFrom(origem => origem.NomeExibicao))
                    .ForMember(destino => destino.Documento, opt => opt.MapFrom(origem => ResolverEFormatarDocumento(origem.Login, origem.Cpf)));

            }
        }
    }
}