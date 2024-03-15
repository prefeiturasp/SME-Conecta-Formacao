using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricao
{
    public class ObterRegistrosDaIncricaoInconsistentesQueryHandler : IRequestHandler<ObterRegistrosDaIncricaoInconsistentesQuery, PaginacaoResultadoDTO<RegistroDaInscricaoInsconsistenteDTO>>
    {
        private readonly IRepositorioImportacaoArquivoRegistro repositorioImportacao;

        public ObterRegistrosDaIncricaoInconsistentesQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioImportacao)
        {
            this.repositorioImportacao = repositorioImportacao ?? throw new ArgumentNullException(nameof(repositorioImportacao));
        }

        public async Task<PaginacaoResultadoDTO<RegistroDaInscricaoInsconsistenteDTO>> Handle(ObterRegistrosDaIncricaoInconsistentesQuery request, CancellationToken cancellationToken)
        {
            var registros = new List<RegistroDaInscricaoInsconsistenteDTO>();
            var registrosComErro = await repositorioImportacao.ObterRegistrosComErro(request.QuantidadeRegistrosIgnorados, request.NumeroRegistros, request.ArquivoId);

            if (registrosComErro.TotalRegistros > 0)
                foreach (var registroErro in registrosComErro.Registros)
                {
                    var registro = JsonConvert.DeserializeObject<RegistroDaInscricaoInsconsistenteDTO>(registroErro.Conteudo);
                    registro.Linha = registroErro.Linha;
                    registro.Erro = registroErro.Erro;
                    registros.Add(registro);
                }

            return new PaginacaoResultadoDTO<RegistroDaInscricaoInsconsistenteDTO>(registros, registrosComErro.TotalRegistros, request.NumeroRegistros);
        }
    }
}
