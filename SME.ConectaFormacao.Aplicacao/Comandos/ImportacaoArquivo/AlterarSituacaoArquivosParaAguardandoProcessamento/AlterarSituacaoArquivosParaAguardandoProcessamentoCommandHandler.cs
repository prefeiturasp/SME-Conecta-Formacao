using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento
{
    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommandHandler : IRequestHandler<AlterarSituacaoArquivosParaAguardandoProcessamentoCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly IRepositorioImportacaoArquivo repositorioImportacaoArquivo;

        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommandHandler(IMediator mediator, IRepositorioImportacaoArquivo repositorioImportacaoArquivo)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
        }

        public async Task<bool> Handle(AlterarSituacaoArquivosParaAguardandoProcessamentoCommand request, CancellationToken cancellationToken)
        {
            var arquivo = await repositorioImportacaoArquivo.ObterPorId(request.ArquivoImportacaoId);

            if (arquivo.Situacao != SituacaoImportacaoArquivo.Validado)
                    throw new NegocioException(MensagemNegocio.SITUACAO_DO_ARQUIVO_DEVE_SER_VALIDADO);

            arquivo.Situacao = SituacaoImportacaoArquivo.AguardandoProcessamento;

            await repositorioImportacaoArquivo.Atualizar(arquivo);

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.ProcessarArquivoDeImportacaoInscricao, arquivo.Id));

            return true;
        }
    }
}
