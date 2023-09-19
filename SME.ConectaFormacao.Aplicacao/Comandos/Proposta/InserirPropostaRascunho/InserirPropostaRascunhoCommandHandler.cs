using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaRascunhoCommandHandler : IRequestHandler<InserirPropostaRascunhoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public InserirPropostaRascunhoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(InserirPropostaRascunhoCommand request, CancellationToken cancellationToken)
        {
            var proposta = _mapper.Map<Proposta>(request.PropostaDTO);
            proposta.AreaPromotoraId = request.AreaPromotoraId;
            proposta.Situacao = SituacaoProposta.Rascunho;

            var publicosAlvo = _mapper.Map<IEnumerable<PropostaPublicoAlvo>>(request.PropostaDTO.PublicosAlvo);
            var funcoesEspecificas = _mapper.Map<IEnumerable<PropostaFuncaoEspecifica>>(request.PropostaDTO.FuncoesEspecificas);
            var criteriosValidacaoInscricao = _mapper.Map<IEnumerable<PropostaCriterioValidacaoInscricao>>(request.PropostaDTO.CriteriosValidacaoInscricao);
            var vagasRemanecentes = _mapper.Map<IEnumerable<PropostaVagaRemanecente>>(request.PropostaDTO.VagasRemanecentes);

            var transacao = _transacao.Iniciar();

            try
            {
                var id = await _repositorioProposta.Inserir(transacao, proposta);

                if (publicosAlvo.Any())
                    await _repositorioProposta.InserirPublicosAlvo(transacao, id, publicosAlvo);

                if (funcoesEspecificas.Any())
                    await _repositorioProposta.InserirFuncoesEspecificas(transacao, id, funcoesEspecificas);

                if (criteriosValidacaoInscricao.Any())
                    await _repositorioProposta.InserirCriteriosValidacaoInscricao(transacao, id, criteriosValidacaoInscricao);

                if (vagasRemanecentes.Any())
                    await _repositorioProposta.InserirVagasRemanecentes(transacao, id, vagasRemanecentes);

                await _mediator.Send(new ValidarArquivoImagemDivulgacaoPropostaCommand(proposta.ArquivoImagemDivulgacaoId), cancellationToken);

                transacao.Commit();

                return id;
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
