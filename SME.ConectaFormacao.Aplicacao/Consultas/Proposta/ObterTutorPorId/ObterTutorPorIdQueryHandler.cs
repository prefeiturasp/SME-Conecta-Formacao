using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTutorPorIdQueryHandler :IRequestHandler<ObterTutorPorIdQuery, PropostaTutor>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterTutorPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PropostaTutor> Handle(ObterTutorPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterPropostaTutorPorId(request.TutorId);
        }
    }
}