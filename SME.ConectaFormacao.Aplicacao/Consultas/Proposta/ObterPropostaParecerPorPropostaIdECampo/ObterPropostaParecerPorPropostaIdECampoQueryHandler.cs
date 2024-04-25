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
                
                if (!perfilLogado.EhPerfilAdminDF())
                {
                    var pareceresDaPropostaDoUsuario = pareceresDaProposta.Where(w => w.CriadoPor.EstaPreenchido() && w.CriadoPor.Equals(usuarioLogado.Login));
                    
                    pareceresDaPropostaDoPerfil = _mapper.Map<IEnumerable<PropostaParecerDTO>>(pareceresDaPropostaDoUsuario);

                    foreach (var propostaParecerFinal in pareceresDaPropostaDoPerfil)
                    {
                        propostaParecerFinal.PodeAlterar = true;
                        propostaParecerFinal.PodeAlterar = true;
                    }
                    
                    podeInserir = !pareceresDaPropostaDoPerfil.Any();

                    if (ehAreaPromotora.NaoEhNulo())
                    {
                        var propostaParecerAreaPromotora = pareceresDaProposta.Where(w => w.CriadoPor.EstaPreenchido() && w.CriadoPor.Equals(usuarioLogado.Login));

                        pareceresDaPropostaDoPerfil = pareceresDaPropostaDoPerfil.Concat(_mapper.Map<IEnumerable<PropostaParecerDTO>>(propostaParecerAreaPromotora));
                    }
                }
                else
                {
                    podeInserir = false;

                    pareceresDaPropostaDoPerfil = _mapper.Map<IEnumerable<PropostaParecerDTO>>(pareceresDaProposta);
                    
                    foreach (var propostaParecerFinal in pareceresDaPropostaDoPerfil)
                        propostaParecerFinal.PodeAlterar = true;
                }

                auditoriaMaisRecente = pareceresDaProposta.MaxBy(o => o.AlteradoEm ?? o.CriadoEm);
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
    }
}