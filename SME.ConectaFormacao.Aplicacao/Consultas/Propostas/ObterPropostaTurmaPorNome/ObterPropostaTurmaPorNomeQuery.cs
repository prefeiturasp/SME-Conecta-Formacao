using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaPorNomeQuery : IRequest<PropostaTurma>
    {
        public ObterPropostaTurmaPorNomeQuery(string propostaTurmaNome, long propostaId)
        {
            PropostaTurmaNome = propostaTurmaNome;
            PropostaId = propostaId;
        }

        public string PropostaTurmaNome { get; }
        public long PropostaId { get; }
    }
}
