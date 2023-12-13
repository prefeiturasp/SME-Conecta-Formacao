using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaResumida
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string AreaPromotora { get; set; }
        public int TipoFormacaoId { get; set; }
        public int FormatoId { get; set; } 
        public string ImagemUrl { get; set; }
        public string NomeArquivo { get; set; }
        public Guid CodigoArquivo { get; set; }
        public DateTime RealizacaoInicio { get; set; }
        public DateTime RealizacaoFim { get; set; }
        public DateTime InscricaoInicio { get; set; }
        public DateTime InscricaoFim { get; set; }

        public bool InscricaoEncerrada
        {
            get
            {
                return DateTimeExtension.HorarioBrasilia().Date > InscricaoFim;
            }
        }
        
        public string Periodo
        {
            get
            {
               return $"{RealizacaoInicio.ToString("dd/MM")} até {RealizacaoFim.ToString("dd/MM")}";
            }
        }
        
        public string TipoFormacao
        {
            get
            {
                return ((TipoFormacao)TipoFormacaoId).Nome();
            }
        }
        public string Formato
        {
            get
            {
                return ((Formato)FormatoId).Nome();
            }
        }
    }
}