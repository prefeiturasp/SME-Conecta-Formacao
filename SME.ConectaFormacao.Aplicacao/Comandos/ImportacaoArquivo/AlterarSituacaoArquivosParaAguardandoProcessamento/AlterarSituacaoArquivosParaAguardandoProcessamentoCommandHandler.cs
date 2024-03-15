using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento
{
    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommandHandler : IRequestHandler<AlterarSituacaoArquivosParaAguardandoProcessamentoCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly ITransacao transacao;
        private readonly IRepositorioImportacaoArquivo repositorioImportacaoArquivo;

        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommandHandler(IMediator mediator, ITransacao transacao, IRepositorioImportacaoArquivo repositorioImportacaoArquivo)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            this.repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
        }

        public async Task<bool> Handle(AlterarSituacaoArquivosParaAguardandoProcessamentoCommand request, CancellationToken cancellationToken)
        {
            var arquivos = await repositorioImportacaoArquivo.ObterImportacaoArquivosValidados(request.PropostaId);

            var transacaoIniciada = transacao.Iniciar();
            try
            {
                foreach (var arquivo in arquivos)
                {
                    arquivo.Situacao = SituacaoImportacaoArquivo.AguardandoProcessamento;

                    await repositorioImportacaoArquivo.Atualizar(arquivo);
                }

                transacaoIniciada.Commit();

                foreach (var arquivo in arquivos)
                {
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.ProcessarArquivoDeImportacaoInscricao, arquivo.Id));
                }
            }
            catch
            {
                transacaoIniciada.Rollback();
                throw;
            }
            finally
            {
                transacaoIniciada.Dispose();
            }

            return true;
        }
    }
}
