using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterImportacaoArquivoPorIdQueryHandler : IRequestHandler<ObterImportacaoArquivoPorIdQuery, ImportacaoArquivoDTO>
    {
        private readonly IRepositorioImportacaoArquivo _repositorioImportacaoArquivo;
        private readonly IMapper _mapper;

        public ObterImportacaoArquivoPorIdQueryHandler(IRepositorioImportacaoArquivo repositorioImportacaoArquivo, IMapper mapper)
        {
            _repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ImportacaoArquivoDTO> Handle(ObterImportacaoArquivoPorIdQuery request, CancellationToken cancellationToken)
        {
            var importacaoArquivo = await _repositorioImportacaoArquivo.ObterPorId(request.Id);

            return _mapper.Map<ImportacaoArquivoDTO>(importacaoArquivo);
        }
    }
}