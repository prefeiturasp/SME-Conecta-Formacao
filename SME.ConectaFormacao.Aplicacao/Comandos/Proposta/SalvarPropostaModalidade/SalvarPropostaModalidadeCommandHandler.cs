using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaModalidadeCommandHandler : IRequestHandler<SalvarPropostaModalidadeCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaModalidadeCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaModalidadeCommand request, CancellationToken cancellationToken)
        {
            var modalidadesAntes = await _repositorioProposta.ObterModalidadesPorId(request.PropostaId);

            var modalidadesInserir = request.Modalidades.Where(w => !modalidadesAntes.Any(a => a.Modalidade == w.Modalidade));
            var modalidadesExcluir = modalidadesAntes.Where(w => !request.Modalidades.Any(a => a.Modalidade == w.Modalidade));

            if (modalidadesInserir.Any())
                await _repositorioProposta.InserirModalidades(request.PropostaId, modalidadesInserir);

            if (modalidadesExcluir.Any())
                await _repositorioProposta.RemoverModalidades(modalidadesExcluir);

            return true;
        }
    }
}
