using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Dominio.Comum
{
    public static class ResolvedorDocumentoUsuario
    {
        public static (string Valor, TipoDocumentoUsuario Tipo) Resolver(string login, string cpf)
        {
            if (login.EhRegistroFuncional())
            {
                return (login, TipoDocumentoUsuario.Rf);
            }

            return (cpf, TipoDocumentoUsuario.Cpf);
        }

        public static string FormatarValor(string valor, TipoDocumentoUsuario tipo)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;

            return tipo switch
            {
                TipoDocumentoUsuario.Rf => valor.AplicarMascaraRf(),
                TipoDocumentoUsuario.Cpf => valor.AplicarMascaraCpf(),
                _ => valor
            };
        }
    }
}
