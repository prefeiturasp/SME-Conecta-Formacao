using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    [ExcludeFromCodeCoverage]
    public class Arquivo : EntidadeBaseAuditavel
    {
        public string Nome { get; set; }
        public Guid Codigo { get; set; }
        public string TipoConteudo { get; set; }
        public TipoArquivo Tipo { get; set; }

        public string NomeArquivoFisico
        {
            get
            {
                return $"{Codigo}{Path.GetExtension(Nome)}";
            }
        }

        public bool EhTemp
        {
            get
            {
                return Tipo == TipoArquivo.Temp;
            }
        }
    }
}

