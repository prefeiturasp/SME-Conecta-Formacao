using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo.ServicosFakes
{
    public class PublicarNaFilaProcessamentoDeRegistroCommandFake : IRequestHandler<PublicarNaFilaRabbitCommand, bool>
    {
        private ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao _useCase;
        public PublicarNaFilaProcessamentoDeRegistroCommandFake(ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao useCase)
        {
            _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
        }

        public async Task<bool> Handle(PublicarNaFilaRabbitCommand request, CancellationToken cancellationToken)
        {
            var mensagem = new MensagemRabbit(JsonConvert.SerializeObject(request.Filtros));

            await _useCase.Executar(mensagem);

            return true;
        }
    }
}
