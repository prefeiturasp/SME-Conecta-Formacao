using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto
{
    public class MetricaMensageria
    {
        public MetricaMensageria(string acao, string rota)
        {
            Acao = acao;
            Rota = rota;
            DataHora = DateTimeExtension.HorarioBrasilia();
        }

        public string Acao { get; private set; }
        public string Rota { get; private set; }
        public DateTime DataHora { get; private set; }
    }
}
