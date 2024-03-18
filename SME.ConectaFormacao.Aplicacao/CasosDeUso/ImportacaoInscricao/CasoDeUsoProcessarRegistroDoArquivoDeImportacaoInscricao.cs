using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao : CasoDeUsoAbstrato, ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao
    {
        public CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDTO>()
                                ?? throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            var arquivo = await mediator.Send(new ObterImportacaoArquivoPorIdQuery(importacaoArquivoRegistro.ImportacaoArquivoId));
            var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaDTO>();

            var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorNomeQuery(importacaoInscricaoCursista.Turma, arquivo.PropostaId))
                    ?? throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var inscricaoManual = new InscricaoManualDTO()
            {
                Cpf = importacaoInscricaoCursista.Cpf,
                RegistroFuncional = importacaoInscricaoCursista.RegistroFuncional,
                ProfissionalRede = importacaoInscricaoCursista.ColaboradorRede.EhColaboradorRede(),
                PropostaTurmaId = propostaTurma.Id
            };

            await mediator.Send(new SalvarInscricaoManualCommand(inscricaoManual));

            await mediator.Send(new AlterarSituacaoRegistroImportacaoArquivoCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Processado));

            await AlterarSituacaoArquivo(importacaoArquivoRegistro.ImportacaoArquivoId);

            return true;
        }

        private async Task AlterarSituacaoArquivo(long importacaoArquivoId)
        {
            if (await mediator.Send(new TodosRegistroForamProcessadosQuery(importacaoArquivoId)))
                await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoId, SituacaoImportacaoArquivo.Processado));
        }
    }
}
