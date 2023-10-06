using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPalavraChaveCommandHandler : IRequestHandler<SalvarPalavraChaveCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPalavraChaveCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPalavraChaveCommand request, CancellationToken cancellationToken)
        {
            var palavrasChavesAntes = await _repositorioProposta.ObterPalavraChavePorId(request.PropostaId);

            var palavrasChavesInserir = request.PalavrasChaves.Where(w => !palavrasChavesAntes.Any(a => a.PalavraChaveId == w.PalavraChaveId));
            var palavrasChavesExcluir = palavrasChavesAntes.Where(w => !request.PalavrasChaves.Any(a => a.PalavraChaveId == w.PalavraChaveId));

            if (palavrasChavesInserir.Any())
                await _repositorioProposta.InserirPalavraChave(request.PropostaId, palavrasChavesInserir);

            if (palavrasChavesExcluir.Any())
                await _repositorioProposta.RemoverPalavrasChaves(palavrasChavesExcluir);

            return true;
        }
    }
}
