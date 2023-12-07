using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaRascunhoCommandHandler : IRequestHandler<AlterarPropostaRascunhoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public AlterarPropostaRascunhoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(AlterarPropostaRascunhoCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);

            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.Situacao = SituacaoProposta.Rascunho;
            propostaDepois.ManterCriador(proposta);
            propostaDepois.AcaoFormativaTexto = proposta.AcaoFormativaTexto;
            propostaDepois.AcaoFormativaLink = proposta.AcaoFormativaLink;

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioProposta.Atualizar(propostaDepois);

                await _mediator.Send(new SalvarPropostaPublicoAlvoCommand(request.Id, propostaDepois.PublicosAlvo), cancellationToken);

                await _mediator.Send(new SalvarPropostaFuncaoEspecificaCommand(request.Id, propostaDepois.FuncoesEspecificas), cancellationToken);

                await _mediator.Send(new SalvarPropostaCriteriosValidacaoInscricaoCommand(request.Id, propostaDepois.CriteriosValidacaoInscricao), cancellationToken);

                await _mediator.Send(new SalvarPropostaVagaRemanecenteCommand(request.Id, propostaDepois.VagasRemanecentes), cancellationToken);

                await _mediator.Send(new SalvarPalavraChaveCommand(request.Id, propostaDepois.PalavrasChaves), cancellationToken);

                await _mediator.Send(new SalvarCriterioCertificacaoCommand(request.Id, propostaDepois.CriterioCertificacao), cancellationToken);

                await _mediator.Send(new SalvarPropostaDreCommand(request.Id, propostaDepois.Dres), cancellationToken);

                await _mediator.Send(new SalvarPropostaTurmaCommand(request.Id, propostaDepois.Turmas), cancellationToken);

                if (proposta.ArquivoImagemDivulgacaoId.GetValueOrDefault() != propostaDepois.ArquivoImagemDivulgacaoId.GetValueOrDefault())
                {
                    await _mediator.Send(new ValidarArquivoImagemDivulgacaoPropostaCommand(propostaDepois.ArquivoImagemDivulgacaoId), cancellationToken);

                    if (proposta.ArquivoImagemDivulgacaoId.HasValue)
                        await _mediator.Send(new RemoverArquivoPorIdCommand(proposta.ArquivoImagemDivulgacaoId.Value), cancellationToken);
                }

                transacao.Commit();

                return request.Id;
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
