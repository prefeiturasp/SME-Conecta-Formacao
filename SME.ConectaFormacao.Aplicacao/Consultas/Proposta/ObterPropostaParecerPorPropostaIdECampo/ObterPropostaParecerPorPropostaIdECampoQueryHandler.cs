using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaParecerPorPropostaIdECampoQueryHandler : IRequestHandler<ObterPropostaParecerPorPropostaIdECampoQuery, PropostaParecerCompletoDTO>
    {
        private readonly IRepositorioPropostaParecer _repositorioPropostaParecer;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ObterPropostaParecerPorPropostaIdECampoQueryHandler(IRepositorioPropostaParecer repositorioPropostaParecer,IMapper mapper,IMediator mediator)
        {
            _repositorioPropostaParecer = repositorioPropostaParecer ?? throw new ArgumentNullException(nameof(repositorioPropostaParecer));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<PropostaParecerCompletoDTO> Handle(ObterPropostaParecerPorPropostaIdECampoQuery request, CancellationToken cancellationToken)
        {
            var pareceresDaProposta = await _repositorioPropostaParecer.ObterPorPropostaIdECampo(request.PropostaId,request.CampoParecer);

            PropostaParecer auditoriaMaisRecente = new ();

            var pareceresDaPropostaDoPerfil = Enumerable.Empty<PropostaParecerDTO>();

            var podeInserir = true;

            if (pareceresDaProposta.Any())
            {
                var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);
                
                var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);
                
                var ehAreaPromotora = await _mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado), cancellationToken);

                if (perfilLogado.EhParecerista())
                {
                    var pareceresDaPropostaDoUsuario = pareceresDaProposta.Where(w => w.CriadoPor.EstaPreenchido() && w.CriadoPor.Equals(usuarioLogado.Login));
                    
                    pareceresDaPropostaDoPerfil = MapearParaDTO(pareceresDaPropostaDoUsuario);

                    DefinirPodeAlterar(pareceresDaPropostaDoPerfil);
                    
                    podeInserir = !pareceresDaPropostaDoPerfil.Any();
                    
                    auditoriaMaisRecente = DefinirAuditoriaMaisRecente(pareceresDaPropostaDoUsuario);
                }
                else if(perfilLogado.EhPerfilAdminDF() || ehAreaPromotora.NaoEhNulo())
                {
                    podeInserir = false;

                    pareceresDaPropostaDoPerfil = MapearParaDTO(pareceresDaProposta);
                    
                    DefinirPodeAlterar(pareceresDaPropostaDoPerfil,perfilLogado.EhPerfilAdminDF());
                    
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