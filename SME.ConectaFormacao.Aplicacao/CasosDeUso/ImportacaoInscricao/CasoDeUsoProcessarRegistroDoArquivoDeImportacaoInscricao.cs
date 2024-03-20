using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao : CasoDeUsoAbstrato, ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao
    {
        private readonly IConexoesRabbit _conexoesRabbit;
        public CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao(IMediator mediator, IConexoesRabbit conexoesRabbit) : base(mediator)
        {
            _conexoesRabbit = conexoesRabbit ?? throw new ArgumentNullException(nameof(conexoesRabbit));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDTO>()
                                ?? throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaImportacaoDTO>();

            await mediator.Send(new SalvarInscricaoImportacaoCommand(importacaoInscricaoCursista));
            await mediator.Send(new AlterarSituacaoRegistroImportacaoArquivoCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Processado));

            await AlterarSituacaoArquivo(importacaoArquivoRegistro.ImportacaoArquivoId);

            return true;
        }

        private async Task AlterarSituacaoArquivo(long importacaoArquivoId)
        {
            var possuiRegistroValidado = await mediator.Send(new PossuiRegistroPorArquivoSituacaoQuery(importacaoArquivoId, SituacaoImportacaoArquivoRegistro.Validado));
            var possuiRegistrosNaFila = _conexoesRabbit.Get().MessageCount(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem) > 0;

            if (!possuiRegistroValidado || !possuiRegistrosNaFila)
                await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoId, SituacaoImportacaoArquivo.Processado));
        }
    }
}
