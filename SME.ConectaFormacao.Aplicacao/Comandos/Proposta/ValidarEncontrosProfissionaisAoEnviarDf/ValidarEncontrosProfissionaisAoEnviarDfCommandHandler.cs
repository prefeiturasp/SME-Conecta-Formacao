using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarEncontrosProfissionaisAoEnviarDfCommandHandler : IRequestHandler<ValidarEncontrosProfissionaisAoEnviarDfCommand, IEnumerable<string>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarEncontrosProfissionaisAoEnviarDfCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<string>> Handle(ValidarEncontrosProfissionaisAoEnviarDfCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var totalRegentes = await _repositorioProposta.ObterTotalRegentes(request.Proposta.Id);
            var quantidadeDeTurmasComEncontro = await _repositorioProposta.ObterQuantidadeDeTurmasComEncontro(request.Proposta.Id);

            if (quantidadeDeTurmasComEncontro != request.Proposta.QuantidadeTurmas)
                erros.Add(MensagemNegocio.QUANTIDADE_TURMAS_COM_ENCONTRO_DIFERENTE_QUANTIDADE_DE_TURMAS);
            if (request.Proposta.QuantidadeTurmas != totalRegentes)
                erros.Add(MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE);

            return erros;
        }
    }
}