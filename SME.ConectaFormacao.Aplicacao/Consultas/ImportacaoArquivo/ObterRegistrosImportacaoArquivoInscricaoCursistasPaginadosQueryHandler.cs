using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryHandler : IRequestHandler<ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery, PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioInscricaoImportacaoArquivoRegistro;
        private readonly IMapper _mapper;

        public ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioInscricaoImportacaoArquivoRegistro, IMapper mapper)
        {
            _repositorioInscricaoImportacaoArquivoRegistro = repositorioInscricaoImportacaoArquivoRegistro ?? throw new ArgumentNullException(nameof(repositorioInscricaoImportacaoArquivoRegistro));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>> Handle(ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery request, CancellationToken cancellationToken)
        {
            var registrosFiltro = _mapper.Map<IEnumerable<ImportacaoArquivoRegistroDTO>>(await _repositorioInscricaoImportacaoArquivoRegistro.ObterRegistrosImportacaoArquivoInscricaoCursistasPaginados(request.ImportacaoArquivoId, request.NumeroRegistros, request.NumeroPagina));

            return new PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>(registrosFiltro, 0, request.NumeroRegistros);
        }
    }
}