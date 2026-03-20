using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPalavraChaveCommandHandler : IRequestHandler<SalvarPropostaPalavraChaveCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaPalavraChaveCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaPalavraChaveCommand request, CancellationToken cancellationToken)
        {
            var palavrasChavesAntes = await _repositorioProposta.ObterPalavrasChavesPorId(request.PropostaId);

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
