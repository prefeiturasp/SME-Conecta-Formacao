using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaFuncaoEspecificaCommandHandler : IRequestHandler<SalvarPropostaFuncaoEspecificaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaFuncaoEspecificaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaFuncaoEspecificaCommand request, CancellationToken cancellationToken)
        {
            var funcoesEspecificasAntes = await _repositorioProposta.ObterFuncoesEspecificasPorId(request.PropostaId);

            var funcoesEspecificasInserir = request.FuncaoEspecificas.Where(w => !funcoesEspecificasAntes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));
            var funcoesEspecificasExcluir = funcoesEspecificasAntes.Where(w => !request.FuncaoEspecificas.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));

            if (funcoesEspecificasInserir.Any())
                await _repositorioProposta.InserirFuncoesEspecificas(request.PropostaId, funcoesEspecificasInserir);

            if (funcoesEspecificasExcluir.Any())
                await _repositorioProposta.RemoverFuncoesEspecificas(funcoesEspecificasExcluir);

            return true;
        }
    }
}
