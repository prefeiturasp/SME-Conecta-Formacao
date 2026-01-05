using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento
{
    public class ContinuarProcessamentoDasInscricoesImportadasCommandHandler(
        IMediator mediator, IRepositorioImportacaoArquivo repositorioImportacaoArquivo) : 
        IRequestHandler<ContinuarProcessamentoDasInscricoesImportadasCommand, bool>
    {
        public async Task<bool> Handle(ContinuarProcessamentoDasInscricoesImportadasCommand request, CancellationToken cancellationToken)
        {
            var arquivo = await repositorioImportacaoArquivo.ObterPorId(request.ArquivoImportacaoId);

            if (arquivo is null)
                throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO);

            if (arquivo.Situacao != SituacaoImportacaoArquivo.Validado)
                throw new NegocioException(MensagemNegocio.SITUACAO_DO_ARQUIVO_DEVE_SER_VALIDADO);

            arquivo.DefinirSituacao(SituacaoImportacaoArquivo.AguardandoProcessamento);

            await repositorioImportacaoArquivo.Atualizar(arquivo);

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.ProcessarArquivoDeImportacaoInscricao, arquivo.Id));

            return true;
        }
    }
}
