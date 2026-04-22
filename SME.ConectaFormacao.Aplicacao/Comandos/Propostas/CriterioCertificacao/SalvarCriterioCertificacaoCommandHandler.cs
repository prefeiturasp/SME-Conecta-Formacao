using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarCriterioCertificacaoCommandHandler : IRequestHandler<SalvarCriterioCertificacaoCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarCriterioCertificacaoCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarCriterioCertificacaoCommand request, CancellationToken cancellationToken)
        {
            var criterioCertificacaoAntes = await _repositorioProposta.ObterCriterioCertificacaoPorPropostaId(request.PropostaId);

            var criteriosInserir = request.CriterioCertificacaos.Where(w => !criterioCertificacaoAntes.Any(a => a.CriterioCertificacaoId == w.CriterioCertificacaoId));

            var criteriosExcluir = criterioCertificacaoAntes.Where(w => !request.CriterioCertificacaos.Any(a => a.CriterioCertificacaoId == w.CriterioCertificacaoId));

            if (criteriosInserir.Any())
                await _repositorioProposta.InserirCriterioCertificacao(request.PropostaId, criteriosInserir);

            if (criteriosExcluir.Any())
                await _repositorioProposta.RemoverCriterioCertificacao(criteriosExcluir);

            return true;
        }
    }
}