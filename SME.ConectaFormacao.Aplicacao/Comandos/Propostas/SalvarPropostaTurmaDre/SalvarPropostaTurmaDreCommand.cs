using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaDreCommand : IRequest<bool>
    {
        public SalvarPropostaTurmaDreCommand(IEnumerable<PropostaTurmaDre> propostaTurmasDres)
        {
            PropostaTurmasDres = propostaTurmasDres;
        }
        public IEnumerable<PropostaTurmaDre> PropostaTurmasDres { get; }
    }
}
