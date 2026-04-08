using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPublicoAlvoCommandHandler : IRequestHandler<SalvarPropostaPublicoAlvoCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaPublicoAlvoCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaPublicoAlvoCommand request, CancellationToken cancellationToken)
        {
            var publicoAlvoAntes = await _repositorioProposta.ObterPublicoAlvoPorId(request.PropostaId);

            var publicoAlvoInserir = request.PublicosAlvo.Where(w => !publicoAlvoAntes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));
            var publicoAlvoExcluir = publicoAlvoAntes.Where(w => !request.PublicosAlvo.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));

            if (publicoAlvoInserir.Any())
                await _repositorioProposta.InserirPublicosAlvo(request.PropostaId, publicoAlvoInserir);

            if (publicoAlvoExcluir.Any())
                await _repositorioProposta.RemoverPublicosAlvo(publicoAlvoExcluir);

            return true;
        }
    }
}
