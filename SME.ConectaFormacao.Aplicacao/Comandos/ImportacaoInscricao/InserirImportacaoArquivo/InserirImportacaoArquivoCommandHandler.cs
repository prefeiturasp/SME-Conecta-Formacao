using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirImportacaoArquivoCommandHandler : IRequestHandler<InserirImportacaoArquivoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioImportacaoArquivo _repositorioImportacaoArquivo;

        public InserirImportacaoArquivoCommandHandler(IMapper mapper, IRepositorioImportacaoArquivo repositorioImportacaoArquivo)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
        }

        public Task<long> Handle(InserirImportacaoArquivoCommand request, CancellationToken cancellationToken)
        {
            var importacaoArquivo = _mapper.Map<ImportacaoArquivo>(request.ImportacaoArquivo);
            return _repositorioImportacaoArquivo.Inserir(importacaoArquivo);
        }
    }
}
