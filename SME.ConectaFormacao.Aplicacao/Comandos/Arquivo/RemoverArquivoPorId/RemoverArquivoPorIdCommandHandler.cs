using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverArquivoPorIdCommandHandler : IRequestHandler<RemoverArquivoPorIdCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioArquivo _repositorioArquivo;

        public RemoverArquivoPorIdCommandHandler(IMediator mediator, IRepositorioArquivo repositorioArquivo)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
        }

        public async Task<bool> Handle(RemoverArquivoPorIdCommand request, CancellationToken cancellationToken)
        {
            var arquivo = await _repositorioArquivo.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO, System.Net.HttpStatusCode.NotFound);

            await _repositorioArquivo.Remover(arquivo);
            await _mediator.Send(new RemoverArquivoServicoArmazenamentoCommand(arquivo.NomeArquivoFisico), cancellationToken);

            return true;
        }
    }
}
