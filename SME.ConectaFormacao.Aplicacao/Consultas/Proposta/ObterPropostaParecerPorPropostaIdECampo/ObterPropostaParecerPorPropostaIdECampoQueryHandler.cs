using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaParecerPorPropostaIdECampoQueryHandler : IRequestHandler<ObterPropostaParecerPorPropostaIdECampoQuery, PropostaParecerCompletoDTO>
    {
        private readonly IRepositorioPropostaParecer _repositorioPropostaParecer;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ObterPropostaParecerPorPropostaIdECampoQueryHandler(IRepositorioPropostaParecer repositorioPropostaParecer,IMapper mapper,IMediator mediator,IRepositorioProposta repositorioProposta)
        {
            _repositorioPropostaParecer = repositorioPropostaParecer ?? throw new ArgumentNullException(nameof(repositorioPropostaParecer));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<PropostaParecerCompletoDTO> Handle(ObterPropostaParecerPorPropostaIdECampoQuery request, CancellationToken cancellationToken)
        {
            var pareceresDaProposta = await _repositorioPropostaParecer.ObterPorPropostaIdECampo(request.PropostaId,request.CampoParecer);
            
            var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);
            
            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);
            
            var proposta = await _repositorioProposta.ObterPorId(request.PropostaId);
            
            var pareceristaProposta = await _mediator.Send(new ObterPareceristasAdicionadosNaPropostaQuery(proposta.Id), cancellationToken);

            var souPareceristaDaProposta = pareceristaProposta.Any(a => a.RegistroFuncional.Equals(usuarioLogado.Login));
            
            if (pareceresDaProposta.Any())
            {
                var ehAreaPromotora = await _mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado), cancellationToken);
                
                if (perfilLogado.EhPerfilParecerista())
                    return ObterPareceresDaPropostaDoPerfilParecerista(pareceresDaProposta, usuarioLogado, proposta, souPareceristaDaProposta);
                
                if(perfilLogado.EhPerfilAdminDF() || ehAreaPromotora.NaoEhNulo())
                    return ObterPareceresDaPropostaPorPerfilAdminDFOuAreaPromotora(pareceresDaProposta, perfilLogado.EhPerfilAdminDF(), proposta.Id);
            }

            return new PropostaParecerCompletoDTO()
            {
                PropostaId = request.PropostaId,
                PodeInserir = perfilLogado.EhPerfilParecerista() && proposta.Situacao.EstaAguardandoAnaliseParecerista() && souPareceristaDaProposta,
                Itens = Enumerable.Empty<PropostaParecerDTO>()
            };
        }

        private PropostaParecerCompletoDTO ObterPareceresDaPropostaPorPerfilAdminDFOuAreaPromotora(IEnumerable<PropostaPareceristaConsideracao> pareceresDaProposta, bool ehPerfilAdminDF, long propostaId)
        {
            pareceresDaProposta = pareceresDaProposta.OrderByDescending(o=> o.AlteradoEm ?? o.CriadoEm);

            var pareceresAguardandoDf = MapearParaDTO(pareceresDaProposta);//TODO .Where(w => w.Situacao.EstaAguardandoAnaliseParecerPeloAdminDF()));

            DefinirPodeAlterar(pareceresAguardandoDf,ehPerfilAdminDF);
                    
            var pareceresAguardandoAP = MapearParaDTO(pareceresDaProposta); //TODO .Where(w => w.Situacao.EstaAguardandoAnaliseParecerPelaAreaPromotora()));
                    
            var pareceresDaPropostaDoPerfil = pareceresAguardandoDf.Concat(pareceresAguardandoAP);

            if (!ehPerfilAdminDF && pareceresDaPropostaDoPerfil.Any())
            {
                foreach (var propostaParecerDto in pareceresDaPropostaDoPerfil)
                    propostaParecerDto.Auditoria = null;
            }
            
            return new PropostaParecerCompletoDTO()
            {
                PropostaId = propostaId,
                PodeInserir = false,
                Itens = pareceresDaPropostaDoPerfil
            };
        }

        private PropostaParecerCompletoDTO ObterPareceresDaPropostaDoPerfilParecerista(IEnumerable<PropostaPareceristaConsideracao> pareceresDaProposta, Usuario usuarioLogado, Proposta proposta, bool souPareceristaDaProposta)
        {
            IEnumerable<PropostaParecerDTO> pareceresDaPropostaDoPerfil;
            pareceresDaPropostaDoPerfil = MapearParaDTO(pareceresDaProposta.OrderByDescending(o=> o.AlteradoEm ?? o.CriadoEm));
                    
            var pareceresDaPropostaDoUsuarioLogado = pareceresDaProposta;//TODO.Where(w => w.UsuarioPareceristaId == usuarioLogado.Id);

            foreach (var propostaParecerDto in pareceresDaPropostaDoPerfil)
            {
                propostaParecerDto.PodeAlterar = true; //TODO pareceresDaPropostaDoUsuarioLogado.Any(a => a.Id == propostaParecerDto.Id && a.Situacao.EstaPendenteEnvioParecerPeloParecerista());
                
                if (!pareceresDaPropostaDoUsuarioLogado.Any(a => a.Id == propostaParecerDto.Id))
                {
                    propostaParecerDto.Auditoria = null;
                    propostaParecerDto.PodeAlterar = false;
                }
            }
                    
            var podeInserir = proposta.Situacao.EstaAguardandoAnaliseParecerista() && !pareceresDaPropostaDoUsuarioLogado.Any() && souPareceristaDaProposta;
            
            return new PropostaParecerCompletoDTO()
            {
                PropostaId = proposta.Id,
                PodeInserir = podeInserir,
                Itens = pareceresDaPropostaDoPerfil
            };
        }

        private IEnumerable<PropostaParecerDTO> MapearParaDTO(IEnumerable<PropostaPareceristaConsideracao> pareceresDaPropostaDoUsuario)
        {
            return _mapper.Map<IEnumerable<PropostaParecerDTO>>(pareceresDaPropostaDoUsuario);
        }

        private void DefinirPodeAlterar(IEnumerable<PropostaParecerDTO> pareceresDaPropostaDoPerfil, bool podeAlterar = true)
        {
            foreach (var propostaParecerFinal in pareceresDaPropostaDoPerfil)
                propostaParecerFinal.PodeAlterar = podeAlterar;
        }
    }
}