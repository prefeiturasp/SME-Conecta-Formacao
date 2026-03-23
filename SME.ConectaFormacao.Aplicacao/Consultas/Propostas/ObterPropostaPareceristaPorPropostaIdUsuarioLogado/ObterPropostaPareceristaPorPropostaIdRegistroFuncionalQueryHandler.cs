using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQueryHandler : IRequestHandler<ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery, PropostaParecerista>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PropostaParecerista> Handle(ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterPareceristaPorPropostaIdRegistroFuncional(request.PropostaId, request.RegistroFuncional);
        }
    }
}
