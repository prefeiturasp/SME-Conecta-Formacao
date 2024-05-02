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
            
            PropostaParecer auditoriaMaisRecente = new ();

            var pareceresDaPropostaDoPerfil = Enumerable.Empty<PropostaParecerDTO>();

            var podeInserir = false;

            if (pareceresDaProposta.Any())
            {
                var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);
                
                var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);
                
                var ehAreaPromotora = await _mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado), cancellationToken);

                var proposta = await _repositorioProposta.ObterPorId(request.PropostaId);
                
                if (perfilLogado.EhPerfilParecerista())
                {
                    pareceresDaPropostaDoPerfil = MapearParaDTO(pareceresDaProposta.OrderByDescending(o=> o.AlteradoEm ?? o.CriadoEm));
                    
                    var pareceresDaPropostaDoUsuarioLogado = pareceresDaProposta.Where(w => w.UsuarioPareceristaId == usuarioLogado.Id);

                    foreach (var propostaParecerDto in pareceresDaPropostaDoPerfil)
                        propostaParecerDto.PodeAlterar = proposta.Situacao.EstaAguardandoAnaliseParecerista() && pareceresDaPropostaDoUsuarioLogado.Any(a => a.Id == propostaParecerDto.Id && a.Situacao.EstaPendenteEnvioParecerPeloParecerista());
                    
                    podeInserir = proposta.Situacao.EstaAguardandoAnaliseParecerista() && !pareceresDaPropostaDoUsuarioLogado.Any();
                    
                    auditoriaMaisRecente = DefinirAuditoriaMaisRecente(pareceresDaProposta.Where(w=> w.UsuarioPareceristaId == usuarioLogado.Id));
                }
                else if(perfilLogado.EhPerfilAdminDF() || ehAreaPromotora.NaoEhNulo())
                {
                    podeInserir = false;
                    
                    pareceresDaProposta = pareceresDaProposta.OrderByDescending(o=> o.AlteradoEm ?? o.CriadoEm);
                    
                    var pareceresAguardandoDf = MapearParaDTO(pareceresDaProposta.Where(w => w.Situacao.EstaAguardandoAnaliseParecerPeloAdminDF()));

                    if (proposta.Situacao.EstaAguardandoAnaliseParecerDF())
                        DefinirPodeAlterar(pareceresAguardandoDf,perfilLogado.EhPerfilAdminDF());
                    
                    var pareceresAguardandoAP = MapearParaDTO(pareceresDaProposta.Where(w => w.Situacao.EstaAguardandoAnaliseParecerPelaAreaPromotora()));
                    
                    pareceresDaPropostaDoPerfil = pareceresAguardandoDf.Concat(pareceresAguardandoAP);

                    if (perfilLogado.EhPerfilAdminDF() && pareceresDaPropostaDoPerfil.Any())
                        auditoriaMaisRecente = DefinirAuditoriaMaisRecente(pareceresDaProposta);
                }
            }
                
            var propostaParecerCompletoDTO = new PropostaParecerCompletoDTO()
            {
                PropostaId = request.PropostaId,
                PodeInserir = podeInserir,
                Auditoria = _mapper.Map<AuditoriaDTO>(auditoriaMaisRecente),
                Itens = pareceresDaPropostaDoPerfil
            };

            return propostaParecerCompletoDTO;
        }

        private IEnumerable<PropostaParecerDTO> MapearParaDTO(IEnumerable<PropostaParecer> pareceresDaPropostaDoUsuario)
        {
            return _mapper.Map<IEnumerable<PropostaParecerDTO>>(pareceresDaPropostaDoUsuario);
        }

        private PropostaParecer? DefinirAuditoriaMaisRecente(IEnumerable<PropostaParecer> pareceresDaProposta)
        {
            return pareceresDaProposta.MaxBy(o => o.AlteradoEm ?? o.CriadoEm);
        }

        private void DefinirPodeAlterar(IEnumerable<PropostaParecerDTO> pareceresDaPropostaDoPerfil, bool podeAlterar = true)
        {
            foreach (var propostaParecerFinal in pareceresDaPropostaDoPerfil)
                propostaParecerFinal.PodeAlterar = podeAlterar;
        }
    }
}