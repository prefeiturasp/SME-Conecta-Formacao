using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoImportacaoArquivoCommandHandler : IRequestHandler<AlterarSituacaoImportacaoArquivoCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioImportacaoArquivo _repositorioImportacaoArquivo;

        public AlterarSituacaoImportacaoArquivoCommandHandler(IMapper mapper, IRepositorioImportacaoArquivo repositorioImportacaoArquivo)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioImportacaoArquivo = repositorioImportacaoArquivo ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivo));
        }

        public async Task<bool> Handle(AlterarSituacaoImportacaoArquivoCommand request, CancellationToken cancellationToken)
        {
            var importacaoArquivo = await _repositorioImportacaoArquivo.ObterPorId(request.Id);

            if (importacaoArquivo.EhNulo())
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_NAO_LOCALIZADA);

            importacaoArquivo.DefinirSituacao(request.Situacao);
            
            await _repositorioImportacaoArquivo.Atualizar(importacaoArquivo);

            return true;
        }
    }
}
