using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Proposta.SalvarPropostaDre
{
    public class SalvarPropostaDreCommandHandler : IRequestHandler<SalvarPropostaDreCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaDreCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaDreCommand request, CancellationToken cancellationToken)
        {
            var dresAntes = await _repositorioProposta.ObterDrePorId(request.PropostaId);

            var dresInserir = request.Dres.Where(w => !dresAntes.Any(a => a.DreId == w.DreId));
            var dresExcluir = dresAntes.Where(w => !request.Dres.Any(a => a.DreId == w.DreId));

            if (dresInserir.Any())
                await _repositorioProposta.InserirDres(request.PropostaId, dresInserir);

            if (dresExcluir.Any())
                await _repositorioProposta.RemoverDres(dresExcluir);

            return true;
        }
    }
}
