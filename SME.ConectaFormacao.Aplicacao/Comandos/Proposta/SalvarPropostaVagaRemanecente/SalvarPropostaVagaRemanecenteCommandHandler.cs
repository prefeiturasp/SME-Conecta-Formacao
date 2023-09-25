using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaVagaRemanecenteCommandHandler : IRequestHandler<SalvarPropostaVagaRemanecenteCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaVagaRemanecenteCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaVagaRemanecenteCommand request, CancellationToken cancellationToken)
        {
            var vagasRemanecentesAntes = await _repositorioProposta.ObterVagasRemacenentesPorId(request.PropostaId);

            var vagasRemanecentesInserir = request.VagasRemanecentes.Where(w => !vagasRemanecentesAntes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));
            var vagasRemanecentesExcluir = vagasRemanecentesAntes.Where(w => !request.VagasRemanecentes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));

            if (vagasRemanecentesInserir.Any())
                await _repositorioProposta.InserirVagasRemanecentes(request.PropostaId, vagasRemanecentesInserir);

            if (vagasRemanecentesExcluir.Any())
                await _repositorioProposta.RemoverVagasRemanecentes(vagasRemanecentesExcluir);

            return true;
        }
    }
}
