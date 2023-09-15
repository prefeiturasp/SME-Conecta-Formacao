using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaCommandHandler : IRequestHandler<InserirPropostaCommand, long>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;

        public InserirPropostaCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long> Handle(InserirPropostaCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);
            await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);

            var proposta = _mapper.Map<Proposta>(request.PropostaDTO);
            proposta.AreaPromotoraId = request.AreaPromotoraId;
            proposta.Situacao = SituacaoProposta.Ativo;

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
