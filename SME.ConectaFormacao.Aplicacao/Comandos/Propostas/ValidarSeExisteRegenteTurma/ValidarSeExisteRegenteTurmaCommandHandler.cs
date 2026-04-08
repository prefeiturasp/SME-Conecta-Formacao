using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeExisteRegenteTurmaCommandHandler : IRequestHandler<ValidarSeExisteRegenteTurmaCommand, string>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarSeExisteRegenteTurmaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<string> Handle(ValidarSeExisteRegenteTurmaCommand request, CancellationToken cancellationToken)
        {
            var obterTotalTurmasRegentes = await _repositorioProposta.ObterTotalTurmasRegentes(request.PropostaId);

            if (request.QuantidadeTurmas != obterTotalTurmasRegentes)
                return MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE;

            return string.Empty;
        }
    }
}