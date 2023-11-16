using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaTutor : CasoDeUsoAbstrato, ICasoDeUsoSalvarPropostaTutor
    {
        public CasoDeUsoSalvarPropostaTutor(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(long id, PropostaTutorDTO propostaTutorDto)
        {
            return await mediator.Send(new SalvarPropostaTutorCommand(id, propostaTutorDto));
        }
    }
}