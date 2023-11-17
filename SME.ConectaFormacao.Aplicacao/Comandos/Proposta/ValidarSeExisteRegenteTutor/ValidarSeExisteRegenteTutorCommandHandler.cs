using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeExisteRegenteTutorCommandHandler : IRequestHandler<ValidarSeExisteRegenteTutorCommand, string>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarSeExisteRegenteTutorCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<string> Handle(ValidarSeExisteRegenteTutorCommand request, CancellationToken cancellationToken)
        {
            var totalRegentes = await _repositorioProposta.ObterTotalRegentes(request.PropostaId);
            if (totalRegentes < request.QuantidadeTurmas)
                   return MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE;
            return string.Empty;
        }
    }
}