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
    public class ObterPropostaCompletaPorIdQueryHandler : IRequestHandler<ObterPropostaCompletaPorIdQuery, PropostaCompletoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IRepositorioArquivo _repositorioArquivo;
        private readonly IRepositorioPropostaMovimentacao _repositorioPropostaMovimentacao;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly IMediator _mediator;

        public ObterPropostaCompletaPorIdQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta,
            IRepositorioArquivo repositorioArquivo, IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao,
            IRepositorioAreaPromotora repositorioAreaPromotora, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
            _repositorioPropostaMovimentacao = repositorioPropostaMovimentacao ?? throw new ArgumentNullException(nameof(repositorioPropostaMovimentacao));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<PropostaCompletoDTO> Handle(ObterPropostaCompletaPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id);
            if (proposta == null || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            proposta.Dres = await _repositorioProposta.ObterDrePorId(request.Id);
            proposta.PublicosAlvo = await _repositorioProposta.ObterPublicoAlvoPorId(request.Id);
            proposta.FuncoesEspecificas = await _repositorioProposta.ObterFuncoesEspecificasPorId(request.Id);
            proposta.CriteriosValidacaoInscricao = await _repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.Id);
            proposta.VagasRemanecentes = await _repositorioProposta.ObterVagasRemacenentesPorId(request.Id);
            proposta.PalavrasChaves = await _repositorioProposta.ObterPalavrasChavesPorId(request.Id);
            proposta.Modalidades = await _repositorioProposta.ObterModalidadesPorId(request.Id);
            proposta.AnosTurmas = await _repositorioProposta.ObterAnosTurmasPorId(request.Id);
            proposta.ComponentesCurriculares = await _repositorioProposta.ObterComponentesCurricularesPorId(request.Id);
            proposta.CriterioCertificacao = await _repositorioProposta.ObterCriterioCertificacaoPorPropostaId(request.Id);
            proposta.Turmas = await _repositorioProposta.ObterTurmasPorId(request.Id);
            proposta.TiposInscricao = await _repositorioProposta.ObterTiposInscricaoPorId(request.Id);
            proposta.Movimentacao = await _repositorioPropostaMovimentacao.ObterUltimoParecerPropostaId(request.Id);
            proposta.AreaPromotora = await _repositorioAreaPromotora.ObterPorId(proposta.AreaPromotoraId);
            proposta.UltimaJustificativaDevolucao = await _repositorioPropostaMovimentacao.ObterUltimaJustificativaDevolucao(request.Id);
            proposta.Pareceristas = await _repositorioProposta.ObterPareceristasPorId(request.Id);
  
            foreach (var turma in proposta.Turmas)
                turma.Dres = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(turma.Id);
            
            var propostaCompletaDTO = _mapper.Map<PropostaCompletoDTO>(proposta);
            propostaCompletaDTO.Auditoria = _mapper.Map<AuditoriaDTO>(proposta);
            propostaCompletaDTO.AreaPromotora = _mapper.Map<PropostaAreaPromotoraDTO>(proposta.AreaPromotora);

            var perfilLogado = await _mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia());
            var usuarioLogado = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            var consideracoes = await _repositorioProposta.ObterPropostaPareceristaConsideracaoPorId(proposta.Id);
            var estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf = proposta.Situacao.EstaAguardandoAnaliseParecerPelaDFOuAreaPromotoraOuAnaliseFinalPelaDF();
            var ehPareceristaDaProposta = perfilLogado.EhPerfilParecerista() && proposta.Pareceristas.Any(a => a.RegistroFuncional.Equals(usuarioLogado.Login));
            var possuiPareceristasNaProposta = proposta.Pareceristas.Any();
            var ehParecerista = perfilLogado.EhPerfilParecerista();
            var ehAdminDF = perfilLogado.EhPerfilAdminDF();
            var podeAprovarRecusar = PodeAprovarRecusar(ehParecerista, consideracoes, usuarioLogado, proposta, ehAdminDF);
            var ehAreaPromotora = await EhPerfilAreaPromotora(perfilLogado);
            var totalDeConsideracoes = ObterTotalDePareceresPorCampo(consideracoes, ehAdminDF, proposta.Pareceristas, ehAreaPromotora);
            
            propostaCompletaDTO.EhParecerista = ehParecerista;
            propostaCompletaDTO.EhAdminDF = ehAdminDF;
            propostaCompletaDTO.EhAreaPromotora = ehAreaPromotora;
            propostaCompletaDTO.TotalDeConsideracoes = totalDeConsideracoes;
            propostaCompletaDTO.ExibirConsideracoes = PodeExibirParecer(ehAdminDF, possuiPareceristasNaProposta, estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf, ehPareceristaDaProposta, ehAreaPromotora,totalDeConsideracoes.Count());
            propostaCompletaDTO.PodeEnviar = PodeEnviar(proposta, possuiPareceristasNaProposta, ehAdminDF, ehAreaPromotora);
            propostaCompletaDTO.PodeEnviarConsideracoes = await PodeEnviarParecer(perfilLogado, consideracoes, usuarioLogado.Login, proposta);
            propostaCompletaDTO.QtdeLimitePareceristaProposta = await ObterParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta);
            propostaCompletaDTO.PodeAprovar = podeAprovarRecusar;
            propostaCompletaDTO.PodeRecusar = podeAprovarRecusar;
            propostaCompletaDTO.LabelAprovar = ehParecerista ? "Sugerir aprovação" : "Aprovar";
            propostaCompletaDTO.LabelRecusar = ehParecerista ? "Sugerir recusa" : "Recusar";
            
            if (!proposta.ArquivoImagemDivulgacaoId.HasValue) return propostaCompletaDTO;
            
            var arquivo = await _repositorioArquivo.ObterPorId(proposta.ArquivoImagemDivulgacaoId.Value);
            propostaCompletaDTO.ArquivoImagemDivulgacao = _mapper.Map<PropostaImagemDivulgacaoDTO>(arquivo);

            propostaCompletaDTO.DesativarAnoEhComponente = DesativarAnoEhComponente(proposta);
            
            return propostaCompletaDTO;
        }

        private static bool DesativarAnoEhComponente(Proposta proposta)
        {
            return (proposta.PublicoAlvoOutros.PossuiElementos() || proposta.PublicosAlvo.Any()) 
                   && (proposta.FuncaoEspecificaOutros.PossuiElementos() || proposta.FuncoesEspecificas.Any());
        }

        private bool PodeAprovarRecusar(bool ehParecerista, IEnumerable<PropostaPareceristaConsideracao> consideracoes, Usuario usuarioLogado, Proposta proposta, bool ehAdminDF)
        {
            if (ehParecerista)
                return !consideracoes.Any(a => a.CriadoLogin.Equals(usuarioLogado.Login))
                       && proposta.Pareceristas.Any(a => a.RegistroFuncional.Equals(usuarioLogado.Login) && a.Situacao.EstaAguardandoValidacao())
                       || proposta.Situacao.EstaAguardandoReanaliseParecerista();
            
            return ehAdminDF && (proposta.Situacao.EstaAguardandoAnaliseParecerFinalPelaDF() || !consideracoes.Any());
        }

        private bool PodeExibirParecer(bool ehAdminDF, bool possuiPareceristasNaProposta, bool estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf, 
            bool ehPareceristaDaProposta, bool ehAreaPromotora, int totalDeConsideracoes)
        {
            if (!possuiPareceristasNaProposta)
                return false;

            if (ehPareceristaDaProposta)
                return true;

            return (ehAdminDF || ehAreaPromotora) && estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf && totalDeConsideracoes > 0;
        }

        private async Task<bool> EhPerfilAreaPromotora(Guid usuarioLogado)
        {
            return (await _mediator.Send(new ObterPerfilAreaPromotoraQuery(usuarioLogado))).NaoEhNulo();
        }

        private static IEnumerable<PropostaTotalConsideracaoDTO> ObterTotalDePareceresPorCampo(IEnumerable<PropostaPareceristaConsideracao> propostaPareceres, bool ehPerfilAdminDF
            , IEnumerable<PropostaParecerista> pareceristas, bool ehAreaPromotora)
        {
            if (ehPerfilAdminDF || ehAreaPromotora)
            {
                var pareceristasEnviados = pareceristas.Where(w => w.Situacao.EstaEnviada()).Select(s => s.Id);
                propostaPareceres = propostaPareceres.Where(w => pareceristasEnviados.Contains(w.PropostaPareceristaId));
            }
           
            return propostaPareceres.GroupBy(g => g.Campo).Select(s => new PropostaTotalConsideracaoDTO()
            {
                Campo = s.Key,
                Quantidade = s.Count()
            });
        }

        private async Task<int> ObterParametroSistema(TipoParametroSistema qtdeLimitePareceristaProposta)
        {
            var parametro = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(qtdeLimitePareceristaProposta, DateTimeExtension.HorarioBrasilia().Year));
            return int.Parse(parametro.Valor);
        }

        private bool PodeEnviar(Proposta proposta, bool possuiPareceristasNaProposta, bool ehAdminDF, bool ehAreaPromotora)
        {
            if (ehAdminDF && proposta.Situacao.EstaAguardandoAnaliseDf() 
                || ehAreaPromotora && proposta.Situacao.EstaAnaliseParecerPelaAreaPromotora())
                return possuiPareceristasNaProposta;
            
            return proposta.Situacao.EstaCadastrada() || proposta.Situacao.EstaDevolvida();
        }

        private async Task<bool> PodeEnviarParecer(Guid usuarioLogado, IEnumerable<PropostaPareceristaConsideracao> consideracoes, string usuarioLogin, Proposta proposta)
        {
            if (usuarioLogado.EhPerfilParecerista())
                return proposta.Situacao.EstaAguardandoAnaliseParecerista()
                       && consideracoes.Any(a => a.CriadoLogin.Equals(usuarioLogin))
                       && proposta.Pareceristas.Any(a => a.RegistroFuncional.Equals(usuarioLogin) && a.Situacao.EstaAguardandoValidacao());
            
            return usuarioLogado.EhPerfilAdminDF() 
                   && proposta.Situacao.EstaAguardandoAnaliseParecerDF()
                   && proposta.Pareceristas.All(a => a.Situacao.EstaEnviada());
        }
    }
}
