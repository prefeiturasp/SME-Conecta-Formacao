using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommandHandler : IRequestHandler<ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand, List<string>>
    {
        private readonly IRepositorioCriterioValidacaoInscricao _repositorioCriterioValidacaoInscricao;

        public ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommandHandler(IRepositorioCriterioValidacaoInscricao repositorioCriterioValidacaoInscricao)
        {
            _repositorioCriterioValidacaoInscricao = repositorioCriterioValidacaoInscricao ?? throw new ArgumentNullException(nameof(repositorioCriterioValidacaoInscricao));
        }

        public async Task<List<string>> Handle(ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var preenchidoPublicoAlvo = request.PublicosAlvoDaProposta.PossuiElementos();
            var preenchidoFuncoesEspecificas = request.FuncoesEspecificasDaProposta.PossuiElementos();
            var preenchidoModalidadesAnosTurmaComponentesCurriculares = request.ModalidadesDaProposta.PossuiElementos()
                                                                        && request.AnosTurmaDaProposta.PossuiElementos()
                                                                        && request.ComponentesCurricularesDaProposta.PossuiElementos();

            if (!preenchidoPublicoAlvo && !preenchidoFuncoesEspecificas && !preenchidoModalidadesAnosTurmaComponentesCurriculares)
                throw new NegocioException(MensagemNegocio.PROPOSTA_CRITERIO_VALIDACAO_PUBLICO_ALVO_ANO_TURMA_COMPONENTE_CURRICULAR);

            return erros;
        }
    }
}
