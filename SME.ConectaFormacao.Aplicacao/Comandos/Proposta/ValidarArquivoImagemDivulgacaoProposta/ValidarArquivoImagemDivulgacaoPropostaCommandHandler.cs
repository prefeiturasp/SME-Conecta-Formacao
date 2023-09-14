using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarArquivoImagemDivulgacaoPropostaCommandHandler : IRequestHandler<ValidarArquivoImagemDivulgacaoPropostaCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioArquivo _repositorioArquivo;

        public ValidarArquivoImagemDivulgacaoPropostaCommandHandler(IMediator mediator, IRepositorioArquivo repositorioArquivo)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
        }

        public async Task<bool> Handle(ValidarArquivoImagemDivulgacaoPropostaCommand request, CancellationToken cancellationToken)
        {
            if (request.ArquivoImagemDivulgacaoId != null)
            {
                var arquivo = await _repositorioArquivo.ObterPorId(request.ArquivoImagemDivulgacaoId.Value) ?? throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO, System.Net.HttpStatusCode.NotFound);

                arquivo.Tipo = TipoArquivo.ImagemDivulgacaoProposta;
                await _repositorioArquivo.Atualizar(arquivo);
                await _mediator.Send(new MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand(arquivo.NomeArquivoFisico), cancellationToken);
            }

            return true;
        }
    }
}
