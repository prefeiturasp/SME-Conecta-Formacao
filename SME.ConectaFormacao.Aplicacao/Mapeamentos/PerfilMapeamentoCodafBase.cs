using AutoMapper;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public abstract class PerfilMapeamentoCodafBase : Profile
    {
        protected static string ResolverEFormatarDocumento(Inscricao? inscricao) =>
            inscricao?.Usuario != null
                ? ResolverEFormatarDocumento(inscricao.Usuario.Login, inscricao.Usuario.Cpf)
                : string.Empty;

        protected static string ResolverEFormatarDocumento(string login, string cpf)
        {
            if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(login, cpf);
            return ResolvedorDocumentoUsuario.FormatarValor(documento, tipo);
        }

        protected static string ObterExtensaoArquivo(string nomeArquivo) =>
            string.IsNullOrWhiteSpace(nomeArquivo)
                ? string.Empty
                : Path.GetExtension(nomeArquivo).TrimStart('.');
        
        protected static object? ObterRegrasAprovacao(Proposta proposta) =>
            proposta?.CriterioCertificacao != null
                ? CriterioCertificacaoFactory.ConstruirRegras(proposta.CriterioCertificacao.Select(c => c.CriterioCertificacaoId))
                : null;
    }
}