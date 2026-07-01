using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class InscricoesProfile : Profile
    {
        public InscricoesProfile()
        {
            CreateMap<InscricaoDadosCursistaDto, DadosInscricaoCursistaRetornoDto>()
                .ForMember(destino => destino.Documento, opt => opt.MapFrom(origem => ResolverEFormatarDocumento(origem.Login, origem.Cpf)));
        }
        private static string ResolverEFormatarDocumento(string login, string cpf)
        {
            var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(login, cpf);
            return ResolvedorDocumentoUsuario.FormatarValor(documento, tipo);
        }
    }
}