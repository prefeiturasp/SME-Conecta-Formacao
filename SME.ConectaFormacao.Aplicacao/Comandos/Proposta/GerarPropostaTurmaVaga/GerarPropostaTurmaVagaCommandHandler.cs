﻿using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarPropostaTurmaVagaCommandHandler : IRequestHandler<GerarPropostaTurmaVagaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;

        public GerarPropostaTurmaVagaCommandHandler(IRepositorioProposta repositorioProposta, ITransacao transacao)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<bool> Handle(GerarPropostaTurmaVagaCommand request, CancellationToken cancellationToken)
        {
            var turmas = await _repositorioProposta.ObterTurmasPorId(request.PropostaId);
            if (turmas.NaoPossuiElementos())
                throw new NegocioException(MensagemNegocio.NENHUMA_TURMA_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var transacao = _transacao.Iniciar();
            try
            {
                foreach (var turma in turmas)
                {
                    for (int i = 0; i < request.QuantidadeVagasTurma; i++)
                    {
                        await _repositorioProposta.InserirPropostaTurmaVagas(new PropostaTurmaVaga
                        {
                            PropostaTurmaId = turma.Id
                        });
                    }
                }

                transacao.Commit();
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

            return true;
        }
    }
}