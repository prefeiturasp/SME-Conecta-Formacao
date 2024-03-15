using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricao
{
    public class ObterArquivosInscricaoImportadosQueryHandler : IRequestHandler<ObterArquivosInscricaoImportadosQuery, PaginacaoResultadoDTO<ArquivoInscricaoImportadoDTO>>
    {
        private readonly IRepositorioImportacaoArquivo repositorioImportacao;

        public ObterArquivosInscricaoImportadosQueryHandler(IRepositorioImportacaoArquivo repositorioImportacao)
        {
            this.repositorioImportacao = repositorioImportacao ?? throw new ArgumentNullException(nameof(repositorioImportacao));
        }

        public async Task<PaginacaoResultadoDTO<ArquivoInscricaoImportadoDTO>> Handle(ObterArquivosInscricaoImportadosQuery request, CancellationToken cancellationToken)
        {
            var arquivos = new List<ArquivoInscricaoImportadoDTO>();
            var arquivoImportados = await repositorioImportacao.ObterArquivosInscricaoImportacao(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.PropostaId);

            if (arquivoImportados.TotalRegistros > 0)
                foreach (var arquivo in arquivoImportados.Registros)
                    arquivos.Add(new ArquivoInscricaoImportadoDTO()
                    {
                        Id = arquivo.Id,
                        Nome = arquivo.Nome,
                        Situacao = arquivo.Situacao,
                        TotalProcessados = arquivo.TotalProcessados,
                        TotalRegistros = arquivo.TotalRegistros
                    });

            return new PaginacaoResultadoDTO<ArquivoInscricaoImportadoDTO>(arquivos, arquivoImportados.TotalRegistros, request.NumeroRegistros);
        }
    }
}
