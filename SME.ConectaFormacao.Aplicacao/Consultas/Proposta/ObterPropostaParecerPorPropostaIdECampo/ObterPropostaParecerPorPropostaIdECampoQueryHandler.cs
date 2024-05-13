using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaParecerPorPropostaIdECampoQueryHandler : IRequestHandler<ObterPropostaParecerPorPropostaIdECampoQuery, PropostaPareceristaConsideracaoCompletoDTO>
    {
        private readonly IRepositorioPropostaPareceristaConsideracao _repositorioPropostaPareceristaConsideracao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ObterPropostaParecerPorPropostaIdECampoQueryHandler(IRepositorioPropostaPareceristaConsideracao repositorioPropostaPareceristaConsideracao,IMapper mapper,IMediator mediator,IRepositorioProposta repositorioProposta)
        {
            _repositorioPropostaPareceristaConsideracao = repositorioPropostaPareceristaConsideracao ?? throw new ArgumentNullException(nameof(repositorioPropostaPareceristaConsideracao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<PropostaPareceristaConsideracaoCompletoDTO> Handle(ObterPropostaParecerPorPropostaIdECampoQuery request, CancellationToken cancellationToken)
        {
            var consideracoesDosPareceristas = await _repositorioPropostaPareceristaConsideracao.ObterPorPropostaIdECampo(request.PropostaId,request.CampoConsideracao);
            
            var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);
            
            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);
            
            var proposta = await _repositorioProposta.ObterPorId(request.PropostaId);
            
            var pareceristasDaProposta = await _mediator.Send(new ObterPareceristasAdicionadosNaPropostaQuery(proposta.Id), cancellationToken);

            var souPareceristaDaProposta = pareceristasDaProposta.Any(a => a.RegistroFuncional.Equals(usuarioLogado.Login));

            var pareceristaEstaAguardandoValidacao = PareceristaEstaAguardandoValidacao(pareceristasDaProposta, usuarioLogado.Login);
           
            if (consideracoesDosPareceristas.Any())
            {
                var ehAreaPromotora = await _mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado), cancellationToken);

                if (perfilLogado.EhPerfilParecerista())
                    return ObterConsideracoesDoPerfilParecerista(consideracoesDosPareceristas, usuarioLogado, proposta, souPareceristaDaProposta, pareceristaEstaAguardandoValidacao);
                
                if(perfilLogado.EhPerfilAdminDF() || ehAreaPromotora.NaoEhNulo())
                    return ObterConsideracoesPorPerfilAdminDFOuAreaPromotora(consideracoesDosPareceristas, perfilLogado.EhPerfilAdminDF(), proposta.Id, pareceristasDaProposta);
            }

            return new PropostaPareceristaConsideracaoCompletoDTO()
            {
                PropostaId = request.PropostaId,
                PodeInserir = perfilLogado.EhPerfilParecerista() && proposta.Situacao.EstaAguardandoAnalisePeloParecerista() && souPareceristaDaProposta && pareceristaEstaAguardandoValidacao,
                Itens = Enumerable.Empty<PropostaPareceristaConsideracaoDTO>()
            };
        }

        private bool PareceristaEstaAguardandoValidacao(IEnumerable<PropostaParecerista> pareceristasDaProposta, string usuarioLogado)
        {
            var situacaoDoParecerista = pareceristasDaProposta.FirstOrDefault(a => a.RegistroFuncional.Equals(usuarioLogado))?.Situacao;
                
            return situacaoDoParecerista.HasValue && situacaoDoParecerista.Value.EstaAguardandoValidacao();
        }

        private PropostaPareceristaConsideracaoCompletoDTO ObterConsideracoesPorPerfilAdminDFOuAreaPromotora(IEnumerable<PropostaPareceristaConsideracao> consideracoesDosPareceristas,
            bool ehPerfilAdminDF, long propostaId, IEnumerable<PropostaParecerista> pareceristasDaProposta)
        {
            consideracoesDosPareceristas = consideracoesDosPareceristas.OrderByDescending(o=> o.AlteradoEm ?? o.CriadoEm);

            var consideracoesPareceristasEnviadas = ObterConsideracoesEnviadasPelosPareceristas(consideracoesDosPareceristas, pareceristasDaProposta);
            DefinirPodeAlterar(consideracoesPareceristasEnviadas,ehPerfilAdminDF);
                    
            var consideracoesAguardandoRevalidacao = ObterConsideracoesAguardandoRevalidacao(consideracoesDosPareceristas, pareceristasDaProposta);

            var pareceresDaPropostaDoPerfil = consideracoesPareceristasEnviadas.Concat(consideracoesAguardandoRevalidacao);

            if (!ehPerfilAdminDF && pareceresDaPropostaDoPerfil.Any())
            {
                foreach (var propostaParecerDto in pareceresDaPropostaDoPerfil)
                    propostaParecerDto.Auditoria = null;
            }
            
            return new PropostaPareceristaConsideracaoCompletoDTO()
            {
                PropostaId = propostaId,
                PodeInserir = false,
                Itens = pareceresDaPropostaDoPerfil
            };
        }

        private IEnumerable<PropostaPareceristaConsideracaoDTO> ObterConsideracoesAguardandoRevalidacao(IEnumerable<PropostaPareceristaConsideracao> consideracoesDosPareceristas, IEnumerable<PropostaParecerista> pareceristasDaProposta)
        {
            var pareceristasIDsAguardandoRevalidacao = pareceristasDaProposta.Where(w=> w.Situacao.EstaAguardandoRevalidacao()).Select(s => s.Id);
            
            return MapearParaDTO(consideracoesDosPareceristas.Where(w=> pareceristasIDsAguardandoRevalidacao.Contains(w.PropostaPareceristaId)));
        }

        private IEnumerable<PropostaPareceristaConsideracaoDTO> ObterConsideracoesEnviadasPelosPareceristas(IEnumerable<PropostaPareceristaConsideracao> consideracoesDosPareceristas, IEnumerable<PropostaParecerista> pareceristasDaProposta)
        {
            var pareceristasIDsEnviadas = pareceristasDaProposta.Where(w=> w.Situacao.EstaEnviada()).Select(s => s.Id);
            
            return MapearParaDTO(consideracoesDosPareceristas.Where(w=> pareceristasIDsEnviadas.Contains(w.PropostaPareceristaId)));
        }

        private PropostaPareceristaConsideracaoCompletoDTO ObterConsideracoesDoPerfilParecerista(IEnumerable<PropostaPareceristaConsideracao> consideracoesDosPareceristas, 
            Usuario usuarioLogado, Proposta proposta, bool souPareceristaDaProposta, bool pareceristaEstaAguardandoValidacao)
        {
            IEnumerable<PropostaPareceristaConsideracaoDTO> consideracoesDoPerfilParecerista;
            consideracoesDoPerfilParecerista = MapearParaDTO(consideracoesDosPareceristas.OrderByDescending(o=> o.AlteradoEm ?? o.CriadoEm));
                    
            var consideracoesDoPareceristaLogado = consideracoesDosPareceristas.Where(w => w.CriadoLogin.Equals(usuarioLogado.Login));

            foreach (var consideracaoDoParecista in consideracoesDoPerfilParecerista)
            {
                consideracaoDoParecista.PodeAlterar = consideracoesDoPareceristaLogado.Any(a => a.Id == consideracaoDoParecista.Id) && pareceristaEstaAguardandoValidacao;
                
                if (!consideracoesDoPareceristaLogado.Any(a => a.Id == consideracaoDoParecista.Id))
                {
                    consideracaoDoParecista.Auditoria = null;
                    consideracaoDoParecista.PodeAlterar = false;
                }
            }
                    
            var podeInserir = proposta.Situacao.EstaAguardandoAnalisePeloParecerista() && !consideracoesDoPareceristaLogado.Any() && souPareceristaDaProposta && pareceristaEstaAguardandoValidacao;
            
            return new PropostaPareceristaConsideracaoCompletoDTO()
            {
                PropostaId = proposta.Id,
                PodeInserir = podeInserir,
                Itens = consideracoesDoPerfilParecerista
            };
        }

        private IEnumerable<PropostaPareceristaConsideracaoDTO> MapearParaDTO(IEnumerable<PropostaPareceristaConsideracao> pareceresDaPropostaDoUsuario)
        {
            return _mapper.Map<IEnumerable<PropostaPareceristaConsideracaoDTO>>(pareceresDaPropostaDoUsuario);
        }

        private void DefinirPodeAlterar(IEnumerable<PropostaPareceristaConsideracaoDTO> pareceresDaPropostaDoPerfil, bool podeAlterar = true)
        {
            foreach (var propostaParecerFinal in pareceresDaPropostaDoPerfil)
                propostaParecerFinal.PodeAlterar = podeAlterar;
        }
    }
}