using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoRemoverPropostaTutor: CasoDeUsoAbstrato, ICasoDeUsoRemoverPropostaTutor
    {
        public CasoDeUsoRemoverPropostaTutor(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long tutorId)
        {
            return await mediator.Send(new RemoverPropostaTutorCommand(tutorId));
        }
    }
}