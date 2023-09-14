using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterArquivoBaixarQueryHandler : IRequestHandler<ObterArquivoBaixarQuery, ArquivoBaixadoDTO>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioArquivo _repositorioArquivo;

        public ObterArquivoBaixarQueryHandler(IMediator mediator, IRepositorioArquivo repositorioArquivo)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
        }

        public async Task<ArquivoBaixadoDTO> Handle(ObterArquivoBaixarQuery request, CancellationToken cancellationToken)
        {
            var arquivo = await _repositorioArquivo.ObterPorCodigo(request.Codigo) ??
                throw new NegocioException(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO, System.Net.HttpStatusCode.NotFound);

            var enderecoArquivo = await _mediator.Send(new ObterEnderecoArquivoServicoArmazenamentoQuery(arquivo.NomeArquivoFisico, arquivo.EhTemp), cancellationToken);

            var arquivoFisico = Array.Empty<byte>();
            if (!string.IsNullOrEmpty(enderecoArquivo))
            {
                var response = await new HttpClient().GetAsync(enderecoArquivo, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    arquivoFisico = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            }
            else
                throw new NegocioException(MensagemNegocio.ARQUIVO_FISICO_NAO_ENCONTRADO);

            return new ArquivoBaixadoDTO
            {
                Arquivo = arquivoFisico,
                Nome = arquivo.Nome,
                TipoConteudo = arquivo.TipoConteudo
            };
        }
    }
}
