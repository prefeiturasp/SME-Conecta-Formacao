using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao(IMediator mediator, IConexoesRabbit conexoesRabbit) : 
        CasoDeUsoAbstrato(mediator), ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDto>()
                                            ?? throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            try
            {
                var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaImportacaoDto>()!;

                var inscricao = importacaoInscricaoCursista.Inscricao;

                var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId)) ??
                                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

                var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId)) ??
                               throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

                await mediator.Send(new SalvarInscricaoImportacaoCommand(inscricao));
                await mediator.Send(new AlterarSituacaoRegistroImportacaoArquivoCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Processado));
            }
            catch (Exception e)
            {
                await mediator.Send(new AlterarSituacaoImportacaoArquivoRegistroCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Erro, e.Message));
            }
            finally
            {
                await AlterarSituacaoArquivo(importacaoArquivoRegistro.ImportacaoArquivoId);
            }
            return true;
        }

        private async Task AlterarSituacaoArquivo(long importacaoArquivoId)
        {
            var possuiRegistroValidado = await mediator.Send(new PossuiRegistroPorArquivoSituacaoQuery(importacaoArquivoId, SituacaoImportacaoArquivoRegistro.Validado));
            var possuiRegistrosNaFila = conexoesRabbit.Get().MessageCount(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem) > 0;

            if (!possuiRegistroValidado || !possuiRegistrosNaFila)
                await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoId, SituacaoImportacaoArquivo.Processado));
        }
    }
}
