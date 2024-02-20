using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirArquivoCommandHandler : IRequestHandler<InserirArquivoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioArquivo _repositorioArquivo;

        public InserirArquivoCommandHandler(IMapper mapper, IRepositorioArquivo repositorioArquivo)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
        }

        public Task<long> Handle(InserirArquivoCommand request, CancellationToken cancellationToken)
        {
            var arquivo = _mapper.Map<Arquivo>(request.Arquivo);
            return _repositorioArquivo.Inserir(arquivo);
        }
    }
}
