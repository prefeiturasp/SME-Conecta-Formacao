using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarPropostaTurmaDreCommand : IRequest<bool>
    {
        public SalvarPropostaTurmaDreCommand(IEnumerable<PropostaTurmaDre> propostaTurmasDres)
        {
            PropostaTurmasDres = propostaTurmasDres;
        }
        public IEnumerable<PropostaTurmaDre> PropostaTurmasDres { get; }
    }
}
