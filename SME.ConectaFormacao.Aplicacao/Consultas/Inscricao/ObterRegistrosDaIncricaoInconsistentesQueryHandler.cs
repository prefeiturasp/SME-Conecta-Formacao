using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosDaIncricaoInconsistentesQueryHandler : IRequestHandler<ObterRegistrosDaIncricaoInconsistentesQuery, PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDTO>>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacao;

        public ObterRegistrosDaIncricaoInconsistentesQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacao)
        {
            _repositorioImportacao = repositorioImportacao ?? throw new ArgumentNullException(nameof(repositorioImportacao));
        }

        public async Task<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDTO>> Handle(ObterRegistrosDaIncricaoInconsistentesQuery request, CancellationToken cancellationToken)
        {
            var registros = new List<RegistroDaInscricaoInsconsistenteDTO>();
            var registrosComErro = await _repositorioImportacao.ObterRegistrosComErro(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.ArquivoId);
            var temRegistrosValidados = (await _repositorioImportacao.ObterRegistroPorSituacao(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.ArquivoId, SituacaoImportacaoArquivoRegistro.Validado)).TotalRegistros > 0;

            if (registrosComErro.TotalRegistros > 0)
                foreach (var registroErro in registrosComErro.Registros)
                {
                    var registro = registroErro.Conteudo.JsonParaObjeto<RegistroDaInscricaoInsconsistenteDTO>();
                    registro.Linha = registroErro.Linha;
                    registro.Erro = registroErro.Erro;
                    registros.Add(registro);
                }
            
            return new PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDTO>(registros, registrosComErro.TotalRegistros, request.NumeroRegistros, temRegistrosValidados);
        }
    }
}
