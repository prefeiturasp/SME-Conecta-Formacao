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
            var propostaPareceres = await _repositorioPropostaParecer.ObterPorPropostaIdECampo(request.PropostaId,request.CampoParecer);

            PropostaParecer auditoriaMaisRecente = new ();

            // var podeInserir = false;

            if (propostaPareceres.Any())
            {
                // podeInserir = true;
                
                var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);
                var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);

                if (!perfilLogado.EhPerfilAdminDF())
                {
                    var propostaPareceresUsuario = propostaPareceres.Where(w => w.CriadoPor.EstaPreenchido() && w.CriadoPor.Equals(usuarioLogado.Login));
                    
                    var propostaParecerAreaPromotora = propostaPareceres.Where(w => w.CriadoPor.EstaPreenchido() && w.CriadoPor.Equals(usuarioLogado.Login));

                    propostaPareceres = propostaPareceresUsuario.Concat(propostaParecerAreaPromotora);
                }

                auditoriaMaisRecente = propostaPareceres.MaxBy(o => o.AlteradoEm ?? o.CriadoEm);
            }
                
            var propostaParecerCompletoDTO = new PropostaParecerCompletoDTO()
            {
                PropostaId = request.PropostaId,
                PodeInserir = true,//podeInserir,
                Auditoria = _mapper.Map<AuditoriaDTO>(auditoriaMaisRecente),
                Itens = _mapper.Map<IEnumerable<PropostaParecerDTO>>(propostaPareceres)
            };

            return propostaParecerCompletoDTO;
        }
    }
}