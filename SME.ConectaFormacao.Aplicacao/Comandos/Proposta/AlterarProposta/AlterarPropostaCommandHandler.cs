using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Proposta.CriterioCertificacao;
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
            await _mediator.Send(new ValidarSeExisteRegenteTutorCommand(request.Id), cancellationToken);
            await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);
            await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);
            await _mediator.Send(new ValidarInformacoesGeraisCommand(request.PropostaDTO), cancellationToken);

            /*
             * 3 - Datas
                    Período de realização
                    Cronograma de encontros: Pelo menos um encontro por turma
                    Período de inscrição
             */
            /*
             * 4 - Detalhamento
                    Carga horária presencial: Obrigatório quando a modalidade for presencial
                    Justificativa
                    Objetivos
                    Conteúdo programático
                    Procedimentos metodológicos
                    Referências
                    Palavras-chave: Obrigatório no mínimo 3 e no máximo 5
             */
            /* 5 - Certificação:
                    Curso com certificação: Sim/Não
                    Critérios para certificação: Quando informado "Sim" no campo anterior deverá ser selecionado pelo menos 3 critérios
                    Descrição da atividade obrigatória para certificação: É obrigatório quando selecionado o critério "Realização de atividade obrigatória".
                    Declaro a ação formativa está em conformidade com o Comunicado nº1.043, de 16 de dezembro de 2020
             * 
             */
            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);
            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.Situacao = SituacaoProposta.Ativo;
            propostaDepois.ManterCriador(proposta);

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
