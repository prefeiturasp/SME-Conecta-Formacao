using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoImportacaoCommandHandler : IRequestHandler<SalvarInscricaoImportacaoCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly ITransacao _transacao;

        public SalvarInscricaoImportacaoCommandHandler(IMediator mediator, IRepositorioInscricao repositorioInscricao, ITransacao transacao)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<bool> Handle(SalvarInscricaoImportacaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = request.InscricaoCursistaImportacaoDTO.Inscricao;

            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
            throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            return await PersistirInscricao(proposta.FormacaoHomologada == FormacaoHomologada.Sim, inscricao, proposta.IntegrarNoSGA);
        }

        private async Task<bool> PersistirInscricao(bool formacaoHomologada, Inscricao inscricao, bool integrarNoSGA)
        {
            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioInscricao.Inserir(inscricao);

                if (!formacaoHomologada)
                {
                    bool confirmada = await _repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                    if (!confirmada)
                        throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                    inscricao.Situacao = SituacaoInscricao.Confirmada;
                    await _repositorioInscricao.Atualizar(inscricao);
                }

                transacao.Commit();

                return true;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
