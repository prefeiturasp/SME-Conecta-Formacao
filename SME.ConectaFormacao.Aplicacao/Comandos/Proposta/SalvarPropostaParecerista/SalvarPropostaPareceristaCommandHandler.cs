using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaCommandHandler : IRequestHandler<SalvarPropostaPareceristaCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;

        public SalvarPropostaPareceristaCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta,
            ITransacao transacao, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(SalvarPropostaPareceristaCommand request, CancellationToken cancellationToken)
        {
            var pareceristaAntes = await _repositorioProposta.ObterPropostaPareceristaPorId(request.Parecerista.Id);
            var pareceristaDepois = _mapper.Map<PropostaParecerista>(request.Parecerista);
            
            if (pareceristaDepois.RegistroFuncional.EhNulo())
                throw new NegocioException(MensagemNegocio.RF_INVALIDO);
            
            var transacao = _transacao.Iniciar();
            try
            {
                if (pareceristaAntes != null)
                {
                    pareceristaDepois.PropostaId = request.PropostaId;
                    pareceristaDepois.ManterCriador(pareceristaAntes);
                    await _repositorioProposta.AtualizarPropostaParecerista(pareceristaDepois);
                }
                else await _repositorioProposta.InserirPropostaParecerista(request.PropostaId, pareceristaDepois);
                    
                transacao.Commit();
                return pareceristaDepois.Id;
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