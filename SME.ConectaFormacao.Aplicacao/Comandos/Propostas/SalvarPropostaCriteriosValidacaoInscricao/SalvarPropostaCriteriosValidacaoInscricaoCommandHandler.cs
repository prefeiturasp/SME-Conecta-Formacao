using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaCriteriosValidacaoInscricaoCommandHandler : IRequestHandler<SalvarPropostaCriteriosValidacaoInscricaoCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaCriteriosValidacaoInscricaoCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaCriteriosValidacaoInscricaoCommand request, CancellationToken cancellationToken)
        {
            var criteriosValidacaoInscricaoAntes = await _repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.PropostaId);

            var criteriosValidacaoInscricaoInserir = request.CriteriosValidacaoInscricao.Where(w => !criteriosValidacaoInscricaoAntes.Any(a => a.CriterioValidacaoInscricaoId == w.CriterioValidacaoInscricaoId));
            var criteriosValidacaoInscricaoExcluir = criteriosValidacaoInscricaoAntes.Where(w => !request.CriteriosValidacaoInscricao.Any(a => a.CriterioValidacaoInscricaoId == w.CriterioValidacaoInscricaoId));

            if (criteriosValidacaoInscricaoInserir.Any())
                await _repositorioProposta.InserirCriteriosValidacaoInscricao(request.PropostaId, criteriosValidacaoInscricaoInserir);

            if (criteriosValidacaoInscricaoExcluir.Any())
                await _repositorioProposta.RemoverCriteriosValidacaoInscricao(criteriosValidacaoInscricaoExcluir);

            return true;
        }
    }
}
