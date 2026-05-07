using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class DadosEmissaoCertificadoCodafDto
    {
        public long IdReferencia { get; set; }

        public required string NomeCompleto { get; set; }
        public required string Documento { get; set; }
        public bool TemRf { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; set; }

        public required string NomeFormacao { get; set; }
        public DateTime DataRealizacao { get; set; }
        public int? HorasTotais { get; set; }
        public string? CargaHorariaTotalOutra { get; set; }
        public long? NumeroHomologacao { get; set; }

        public short NumeroComunicado { get; set; }
        public DateTime DataPublicacao { get; set; }

        public string? ConceitoFinal { get; set; }
        public double? PercentualFrequencia { get; set; }

        public string? EmailUsuario { get; set; }

        //aqui, caso o aluno tenha coordenadoria vinculada na DRE, trará ela, caso contrário, trará a DRE
        public string DreCoordenadoria { get; set; } = string.Empty;
        public string TipoFormacao { get; set; } = string.Empty;

    }
}
