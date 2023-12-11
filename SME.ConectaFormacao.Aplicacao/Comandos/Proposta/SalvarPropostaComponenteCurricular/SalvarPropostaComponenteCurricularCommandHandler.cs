using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaComponenteCurricularCommandHandler : IRequestHandler<SalvarPropostaComponenteCurricularCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaComponenteCurricularCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaComponenteCurricularCommand request, CancellationToken cancellationToken)
        {
            var componentesCurricularesAntes = await _repositorioProposta.ObterComponentesCurricularesPorId(request.PropostaId);

            var componentesCurricularesInserir = request.ComponentesCurriculares.Where(w => !componentesCurricularesAntes.Any(a => a.ComponenteCurricularId == w.ComponenteCurricularId));
            var componentesCurricularesExcluir = componentesCurricularesAntes.Where(w => !request.ComponentesCurriculares.Any(a => a.ComponenteCurricularId == w.ComponenteCurricularId));

            if (componentesCurricularesInserir.Any())
                await _repositorioProposta.InserirComponentesCurriculares(request.PropostaId, componentesCurricularesInserir);

            if (componentesCurricularesExcluir.Any())
                await _repositorioProposta.RemoverComponentesCurriculares(componentesCurricularesExcluir);

            return true;
        }
    }
}
