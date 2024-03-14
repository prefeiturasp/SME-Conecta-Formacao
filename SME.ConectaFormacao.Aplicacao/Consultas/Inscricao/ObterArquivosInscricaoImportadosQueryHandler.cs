using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricao
{
    public class ObterArquivosInscricaoImportadosQueryHandler : IRequestHandler<ObterArquivosInscricaoImportadosQuery, PaginacaoResultadoDTO<ArquivoInscricaoImportadoDto>>
    {
        private readonly IRepositorioImportacaoArquivo repositorioImportacao;

        public ObterArquivosInscricaoImportadosQueryHandler(IRepositorioImportacaoArquivo repositorioImportacao)
        {
            this.repositorioImportacao = repositorioImportacao ?? throw new ArgumentNullException(nameof(repositorioImportacao));
        }

        public async Task<PaginacaoResultadoDTO<ArquivoInscricaoImportadoDto>> Handle(ObterArquivosInscricaoImportadosQuery request, CancellationToken cancellationToken)
        {
            var Arquivos = new List<ArquivoInscricaoImportadoDto>();
            var ArquivoImportados = await repositorioImportacao.ObterArquivosInscricaoImportacao(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.PropostaId);

            if (ArquivoImportados.TotalDeRegistros > 0)
                foreach (var arquivo in ArquivoImportados.Arquivos)
                    Arquivos.Add(new ArquivoInscricaoImportadoDto()
                    {
                        Id = arquivo.Id,
                        Nome = arquivo.Nome,
                        Situacao = arquivo.Situacao,
                        TotalProcessados = arquivo.TotalProcessados,
                        TotalRegistros = arquivo.TotalRegistros
                    });

            return new PaginacaoResultadoDTO<ArquivoInscricaoImportadoDto>(Arquivos, ArquivoImportados.TotalDeRegistros, request.NumeroRegistros);
        }
    }
}
