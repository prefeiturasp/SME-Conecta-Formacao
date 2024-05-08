using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
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
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public SalvarPropostaPareceristaCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta,
            IRepositorioUsuario repositorioUsuario, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarPropostaPareceristaCommand request, CancellationToken cancellationToken)
        {
            var pareceristasAntes = await _repositorioProposta.ObterPropostaPareceristaPorId(request.PropostaId);
            var pareceristasDepois = _mapper.Map<IEnumerable<PropostaParecerista>>(request.Pareceristas);
            var possuiPareceristasDepois = pareceristasDepois.Any();
            
            switch (possuiPareceristasDepois)
            {
                case true when pareceristasDepois.Any(a=> a.RegistroFuncional.NaoEstaPreenchido()):
                    throw new NegocioException(string.Format(MensagemNegocio.X_NAO_PREENCHIDO, Constantes.RF));
                case true when pareceristasDepois.Any(a=> a.NomeParecerista.NaoEstaPreenchido()):
                    throw new NegocioException(string.Format(MensagemNegocio.X_NAO_PREENCHIDO, Constantes.NOME_PARECERISTA));
            }

            var qtdeLimitePareceristaProposta = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeLimitePareceristaProposta, DateTimeExtension.HorarioBrasilia().Year));
            
            if(qtdeLimitePareceristaProposta.EhNulo())
                throw new NegocioException(string.Format(MensagemNegocio.PARAMETRO_X_NAO_ENCONTRADO_PARA_ANO_Y, TipoParametroSistema.QtdeLimitePareceristaProposta, DateTimeExtension.HorarioBrasilia().Year));

            var pareceristasInserir = request.Pareceristas.Where(w => !pareceristasAntes.Any(a => a.Id == w.Id));
            var pareceristasExcluir = pareceristasAntes.Where(w => !request.Pareceristas.Any(a => a.Id == w.Id));
            
            var qtdePareceristasDaProposta = pareceristasAntes.Count() - pareceristasExcluir.Count() + pareceristasInserir.Count();
            var limiteMaximoPareceristas = int.Parse(qtdeLimitePareceristaProposta.Valor);
            
            if (qtdePareceristasDaProposta > limiteMaximoPareceristas)
                throw new NegocioException(string.Format(MensagemNegocio.LIMITE_PARECERISTAS_EXCEDIDO_LIMITE_X, limiteMaximoPareceristas));

            if (pareceristasInserir.Any())
                await _repositorioProposta.InserirPareceristas(request.PropostaId, pareceristasInserir);

            if (pareceristasExcluir.Any())
                await _repositorioProposta.RemoverPareceristas(pareceristasExcluir);
                
            return true;
        }
    }
}