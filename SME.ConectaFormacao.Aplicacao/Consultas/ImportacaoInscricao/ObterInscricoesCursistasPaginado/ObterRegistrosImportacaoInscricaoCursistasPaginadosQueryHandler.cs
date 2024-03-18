using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosImportacaoInscricaoCursistasPaginadosQueryHandler : IRequestHandler<ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery, PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioInscricaoImportacaoArquivoRegistro;
        private readonly IMapper _mapper;

        public ObterRegistrosImportacaoInscricaoCursistasPaginadosQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioInscricaoImportacaoArquivoRegistro, IMapper mapper)
        {
            _repositorioInscricaoImportacaoArquivoRegistro = repositorioInscricaoImportacaoArquivoRegistro ?? throw new ArgumentNullException(nameof(repositorioInscricaoImportacaoArquivoRegistro));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>> Handle(ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery request, CancellationToken cancellationToken)
        {
            var registrosPaginados = await _repositorioInscricaoImportacaoArquivoRegistro.ObterRegistroPorSituacao(
                request.NumeroPagina,request.NumeroRegistros, request.ImportacaoArquivoId, request.IgnorarSituacao);

            return new PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>(_mapper.Map<IEnumerable<ImportacaoArquivoRegistroDTO>>(
                registrosPaginados.Registros), registrosPaginados.TotalRegistros, request.NumeroRegistros);
        }
    }
}