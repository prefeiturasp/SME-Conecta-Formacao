using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaTutorCommandHandler : IRequestHandler<RemoverPropostaTutorCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public RemoverPropostaTutorCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(RemoverPropostaTutorCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.ExcluirPropostaTutor(request.TutorId);
            var turmas = await _repositorioProposta.ObterTutorTurmasPorTutorId(request.TutorId);
            if (turmas.Count() > 0)
                await _repositorioProposta.ExcluirPropostaTutorTurma(turmas);
            return true;
        }
    }
}