using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarEncontrosProfissionaisAoEnviarDfCommandHandler(
        IRepositorioProposta repositorioProposta,
        IRepositorioPropostaEncontro repositorioPropostaEncontro) : 
        IRequestHandler<ValidarEncontrosProfissionaisAoEnviarDfCommand, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(ValidarEncontrosProfissionaisAoEnviarDfCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var totalRegentes = await repositorioProposta.ObterTotalRegentes(request.Proposta.Id);
            var quantidadeDeTurmasComEncontro = await repositorioPropostaEncontro.ObterQuantidadeDeTurmasComEncontroAsync(request.Proposta.Id);

            if (quantidadeDeTurmasComEncontro != request.Proposta.QuantidadeTurmas)
                erros.Add(MensagemNegocio.QUANTIDADE_TURMAS_COM_ENCONTRO_DIFERENTE_QUANTIDADE_DE_TURMAS);
            if (request.Proposta.QuantidadeTurmas != totalRegentes)
                erros.Add(MensagemNegocio.QUANTIDADE_TURMAS_COM_REGENTE);

            return erros;
        }
    }
}