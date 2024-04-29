using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaCommandHandler : IRequestHandler<SalvarPropostaPareceristaCommand, bool>
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

        public async Task<bool> Handle(SalvarPropostaPareceristaCommand request, CancellationToken cancellationToken)
        {
            var pareceristaAntes = await _repositorioProposta.ObterPropostaPareceristaPorId(request.PropostaId);
            var pareceristaDepois = _mapper.Map<IEnumerable<PropostaParecerista>>(request.Pareceristas);
            
            if (pareceristaDepois.Any(a=> a.RegistroFuncional.EhNulo()))
                throw new NegocioException(string.Format(MensagemNegocio.X_NAO_PREENCHIDO, Constantes.RF));
            
            if (pareceristaDepois.Any(a=> a.NomeParecerista.EhNulo()))
                throw new NegocioException(string.Format(MensagemNegocio.X_NAO_PREENCHIDO, Constantes.NOME_PARECERISTA));
            
            var pareceristasInserir = request.Pareceristas.Where(w => !pareceristaAntes.Any(a => a.Id == w.Id));
            var pareceristasExcluir = pareceristaAntes.Where(w => !request.Pareceristas.Any(a => a.Id == w.Id));
            
            if (pareceristasInserir.Any())
                await _repositorioProposta.InserirPareceristas(request.PropostaId, pareceristasInserir);

            if (pareceristasExcluir.Any())
                await _repositorioProposta.RemoverPareceristas(pareceristasExcluir);
                
            return true;
        }
    }
}