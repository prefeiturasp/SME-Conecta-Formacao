using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao : CasoDeUsoAbstrato, ICasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao
    {
        private readonly IConexoesRabbit _conexoesRabbit;
        private readonly ITransacao _transacao;
        public CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao(IMediator mediator, IConexoesRabbit conexoesRabbit, ITransacao transacao) : base(mediator)
        {
            _conexoesRabbit = conexoesRabbit ?? throw new ArgumentNullException(nameof(conexoesRabbit));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDTO>()
                                            ?? throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);
            
            var transacao = _transacao.Iniciar();
            
            try
            {
                var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaImportacaoDTO>();

                var inscricao = importacaoInscricaoCursista.Inscricao;

                var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId)) ??
                                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

                var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId)) ??
                               throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
                
                await mediator.Send(new UsuarioEstaInscritoNaPropostaQuery(propostaTurma.PropostaId, inscricao.UsuarioId));
                
                await mediator.Send(new SalvarInscricaoImportacaoCommand(inscricao,proposta.FormacaoHomologada.EstaHomologada()));
                await mediator.Send(new AlterarSituacaoRegistroImportacaoArquivoCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Processado));

                transacao.Commit();
            }
            catch (Exception e)
            {
                transacao.Rollback();
                await mediator.Send(new AlterarSituacaoImportacaoArquivoRegistroCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Erro, e.Message));
            }
            finally
            {
                await AlterarSituacaoArquivo(importacaoArquivoRegistro.ImportacaoArquivoId);
                transacao.Dispose();
            }
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
