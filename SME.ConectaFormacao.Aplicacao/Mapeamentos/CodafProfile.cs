using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafProfile : Profile
    {
        public CodafProfile()
        {
            CreateMap<CodafListaPresenca, CodafListaPresencaDto>()
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(src => src.Proposta.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.Proposta.Id))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.Proposta.NumeroHomologacao))
                .ForMember(dest => dest.Retificacoes, opt => opt.MapFrom(src => src.CodafRetificacoes))
                .ForMember(dest => dest.Anexos, opt => opt.MapFrom(src => src.CodafAnexos))
                .ForMember(dest => dest.RegrasAprovacao, opt => opt.MapFrom(src =>
                    CriterioCertificacaoFactory.ConstruirRegras(
                        src.Proposta.CriterioCertificacao.Select(c => c.CriterioCertificacaoId)
                    )
                ))
                ;
            CreateMap<FiltroListaPresencaCodafDto, FiltroListagemResultadoCodafListaPresencaDto>()
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.CodigoFormacao.ToString()))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.NumeroHomologacao.ToString()))
                .ForMember(dest => dest.Pagina, opt => opt.MapFrom(src => src.NumeroPagina))
                .ForMember(dest => dest.TamanhoPagina, opt => opt.MapFrom(src => src.NumeroRegistros))
                ;
            CreateMap<ListagemResultadoCodafListaPresencaDto, ListaPresencaCodafResumoDto>();

            CreateMap<ResultadoInscritoTurmaCodafListaPresencaDto, CodafInscritoTurmaListaPresencaRetornoDto>()
                .ForMember(destino => destino.Documento, opt => opt.MapFrom(origem => ResolverEFormatarDocumento(origem.Login, origem.Cpf)));

            CreateMap<CodafInscritoListaPresencaSalvarDto, CodafInscricaoListaPresenca>();
            CreateMap<CodafRetificacaoListaPresencaSalvarDto, CodafRetificacaoListaPresenca>();
            CreateMap<CodafRetificacaoListaPresenca, CodafRetificacaoListaPresencaDto>();
            CreateMap<CodafAnexoSalvarDto, CodafAnexo>()
                .ForMember(dest => dest.Extensao, opt => opt.MapFrom(src => src.NomeArquivo.Substring(src.NomeArquivo.LastIndexOf('.') + 1)))
                ;
            CreateMap<CodafAnexo, CodafAnexoDto>().ForMember(dest => dest.UrlDownload, opt => opt.Ignore());
        }
        private static string ResolverEFormatarDocumento(string login, string cpf)
        {
            var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(login, cpf);
            return ResolvedorDocumentoUsuario.FormatarValor(documento, tipo);
        }
    }
}