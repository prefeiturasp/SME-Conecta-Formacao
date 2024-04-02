using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosDaIncricaoInconsistentesQueryHandler : IRequestHandler<ObterRegistrosDaIncricaoInconsistentesQuery, PaginacaoResultadoComMensagemDTO<RegistroDaInscricaoInsconsistenteDTO>>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacao;

        public ObterRegistrosDaIncricaoInconsistentesQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacao)
        {
            _repositorioImportacao = repositorioImportacao ?? throw new ArgumentNullException(nameof(repositorioImportacao));
        }

        public async Task<PaginacaoResultadoComMensagemDTO<RegistroDaInscricaoInsconsistenteDTO>> Handle(ObterRegistrosDaIncricaoInconsistentesQuery request, CancellationToken cancellationToken)
        {
            var registros = new List<RegistroDaInscricaoInsconsistenteDTO>();
            var registrosComErro = await _repositorioImportacao.ObterRegistrosComErro(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.ArquivoId);
            var possuiSucesso = (await _repositorioImportacao.ObterRegistroPorSituacao(0, 1, request.ArquivoId, SituacaoImportacaoArquivoRegistro.Validado)).TotalRegistros > 0;

            if (registrosComErro.TotalRegistros > 0)
                foreach (var registroErro in registrosComErro.Registros)
                {
                    var registro = JsonConvert.DeserializeObject<RegistroDaInscricaoInsconsistenteDTO>(registroErro.Conteudo);
                    registro.Linha = registroErro.Linha;
                    registro.Erro = registroErro.Erro;
                    registros.Add(registro);
                }
            
            return new PaginacaoResultadoComMensagemDTO<RegistroDaInscricaoInsconsistenteDTO>(registros, registrosComErro.TotalRegistros, request.NumeroRegistros,possuiSucesso);
        }
    }
}
