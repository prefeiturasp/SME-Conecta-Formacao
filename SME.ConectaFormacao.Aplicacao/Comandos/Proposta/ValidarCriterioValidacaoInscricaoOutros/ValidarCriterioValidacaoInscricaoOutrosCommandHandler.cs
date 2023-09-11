using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarCriterioValidacaoInscricaoOutrosCommandHandler : IRequestHandler<ValidarCriterioValidacaoInscricaoOutrosCommand>
    {
        private readonly IRepositorioCriterioValidacaoInscricao _repositorioCriterioValidacaoInscricao;

        public ValidarCriterioValidacaoInscricaoOutrosCommandHandler(IRepositorioCriterioValidacaoInscricao repositorioCriterioValidacaoInscricao)
        {
            _repositorioCriterioValidacaoInscricao = repositorioCriterioValidacaoInscricao ?? throw new ArgumentNullException(nameof(repositorioCriterioValidacaoInscricao));
        }

        public async Task Handle(ValidarCriterioValidacaoInscricaoOutrosCommand request, CancellationToken cancellationToken)
        {
            if (request.PropostaCriterioValidacaoInscricaos != null && request.PropostaCriterioValidacaoInscricaos.Any())
            {
                var ids = request.PropostaCriterioValidacaoInscricaos.Select(t => t.CriterioValidacaoInscricaoId).ToArray();
                var existeOpcaoOutros = await _repositorioCriterioValidacaoInscricao.ExisteCriterioValidacaoInscricaoOutros(ids);

                if (existeOpcaoOutros && string.IsNullOrEmpty(request.CriterioValidacaoInscricaoOutros))
                    throw new NegocioException(MensagemNegocio.PROPOSTA_CRITERIO_VALIDACAO_INSCRICAO_OUTROS);
            }
        }
    }
}
