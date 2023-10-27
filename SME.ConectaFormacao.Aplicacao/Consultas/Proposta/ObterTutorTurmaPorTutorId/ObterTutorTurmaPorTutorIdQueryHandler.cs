using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterTutorTurmaPorTutorId
{
    public class ObterTutorTurmaPorTutorIdQueryHandler : IRequestHandler<ObterTutorTurmaPorTutorIdQuery, IEnumerable<PropostaTutorTurma>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterTutorTurmaPorTutorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaTutorTurma>> Handle(ObterTutorTurmaPorTutorIdQuery request, CancellationToken cancellationToken)
        {
            return await  _repositorioProposta.ObterTutorTurmasPorTutorId(request.TutorId);
        }
    }
}