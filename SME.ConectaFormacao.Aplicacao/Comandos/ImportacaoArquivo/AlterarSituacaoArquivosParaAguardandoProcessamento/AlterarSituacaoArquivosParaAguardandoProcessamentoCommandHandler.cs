using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommandHandler : IRequestHandler<AlterarSituacaoArquivosParaAguardandoProcessamentoCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioImportacaoArquivo _repositorioImportacaoArquivo;

        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommandHandler(IMediator mediator, IRepositorioImportacaoArquivo repositorioImportacaoArquivo)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
        }

        public async Task<bool> Handle(AlterarSituacaoArquivosParaAguardandoProcessamentoCommand request, CancellationToken cancellationToken)
        {
            var arquivo = await _repositorioImportacaoArquivo.ObterPorId(request.ArquivoImportacaoId);

            if (arquivo.EhNulo())
                throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO);

            if (arquivo.Situacao != SituacaoImportacaoArquivo.Validado)
                    throw new NegocioException(MensagemNegocio.SITUACAO_DO_ARQUIVO_DEVE_SER_VALIDADO);

            arquivo.Situacao = SituacaoImportacaoArquivo.AguardandoProcessamento;

            await _repositorioImportacaoArquivo.Atualizar(arquivo);

            await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.ProcessarArquivoDeImportacaoInscricao, arquivo.Id));

            return true;
        }
    }
}
