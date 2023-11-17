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
    public class AlterarPropostaCommandHandler : IRequestHandler<AlterarPropostaCommand, long>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;

        public AlterarPropostaCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long> Handle(AlterarPropostaCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);
            await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);
            await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);


            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);
            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.ManterCriador(proposta);
            propostaDepois.AcaoFormativaTexto = proposta.AcaoFormativaTexto;
            propostaDepois.AcaoFormativaLink = proposta.AcaoFormativaLink;
            if (request.PropostaDTO.Situacao == SituacaoProposta.Cadastrada)
            {
                var erros = new List<string>();
                var errosRegente = await _mediator.Send(new ValidarSeExisteRegenteTutorCommand(request.Id, propostaDepois.QuantidadeTurmas ?? 0), cancellationToken);
                if (!string.IsNullOrEmpty(errosRegente))
                    erros.Add(errosRegente);

                var errosInformacoesGerais = await _mediator.Send(new ValidarInformacoesGeraisCommand(request.PropostaDTO), cancellationToken);
                if (errosInformacoesGerais.Any())
                    erros.AddRange(errosInformacoesGerais);

                var errosDatas = await _mediator.Send(new ValidarDatasExistentesNaPropostaCommand(request.Id, request.PropostaDTO), cancellationToken);
                if (errosDatas.Any())
                    erros.AddRange(errosDatas);

                var errosDetalhamento = await _mediator.Send(new ValidarDetalhamentoDaPropostaCommand(request.PropostaDTO), cancellationToken);
                if (errosDetalhamento.Any())
                    erros.AddRange(errosDetalhamento);

                var errosCritériosCertificacao = await _mediator.Send(new ValidarCertificacaoPropostaCommand(request.PropostaDTO), cancellationToken);
                if (errosCritériosCertificacao.Any())
                    erros.AddRange(errosCritériosCertificacao);

                if (erros.Any())
                    throw new NegocioException(erros);
                propostaDepois.Situacao = SituacaoProposta.Cadastrada;
            }
            else
                propostaDepois.Situacao = SituacaoProposta.Ativo;

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