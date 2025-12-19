using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes
{
    public class ObterRegistrosDaIncricaoInconsistentesQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacao) : 
        IRequestHandler<ObterRegistrosDaIncricaoInconsistentesQuery, 
            PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>>
    {
        public async Task<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>> Handle(ObterRegistrosDaIncricaoInconsistentesQuery request, CancellationToken cancellationToken)
        {
            var registros = new List<RegistroDaInscricaoInsconsistenteDto>();
            var registrosComErro = await repositorioImportacao.ObterRegistrosComMensagemDeErro(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.ArquivoId);
            var temRegistrosValidados = (await repositorioImportacao.ObterRegistroPorSituacao(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.ArquivoId, SituacaoImportacaoArquivoRegistro.Validado)).TotalRegistros > 0;

            if (registrosComErro.TotalRegistros > 0)
                foreach (var registroErro in registrosComErro.Registros)
                {
                    var registro = registroErro.Conteudo.JsonParaObjeto<RegistroDaInscricaoInsconsistenteDto>()!;
                    registro.Linha = registroErro.Linha;
                    registro.Erro = registroErro.Erro;
                    registros.Add(registro);
                }

            return new PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>(registros, registrosComErro.TotalRegistros, request.NumeroRegistros, temRegistrosValidados);
        }
    }
}
