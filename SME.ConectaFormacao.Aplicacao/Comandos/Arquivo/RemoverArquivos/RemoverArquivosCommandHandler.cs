using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverArquivosCommandHandler : IRequestHandler<RemoverArquivosCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioArquivo _repositorioArquivo;

        public RemoverArquivosCommandHandler(IMediator mediator, IRepositorioArquivo repositorioArquivo)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
        }

        public async Task<bool> Handle(RemoverArquivosCommand request, CancellationToken cancellationToken)
        {
            foreach (var arquivo in request.Arquivos)
            {
                await _repositorioArquivo.Remover(arquivo);
                await _mediator.Send(new RemoverArquivoServicoArmazenamentoCommand(arquivo.NomeArquivoFisico), cancellationToken);
            }

            return true;
        }
    }
}
