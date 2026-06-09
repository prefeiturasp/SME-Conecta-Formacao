using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGrupoPeriodoPorPropostaId;
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
    public class ObterPropostaCompletaPorIdQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta,
        IRepositorioArquivo repositorioArquivo, IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao,
        IRepositorioAreaPromotora repositorioAreaPromotora, IMediator mediator) : IRequestHandler<ObterPropostaCompletaPorIdQuery, PropostaCompletoDTO>
    {
        public async Task<PropostaCompletoDTO> Handle(ObterPropostaCompletaPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await repositorioProposta.ObterPorId(request.Id);
            if (proposta == null || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            proposta.Dres = await repositorioProposta.ObterDrePorId(request.Id);
            proposta.PublicosAlvo = await repositorioProposta.ObterPublicoAlvoPorId(request.Id);
            proposta.FuncoesEspecificas = await repositorioProposta.ObterFuncoesEspecificasPorId(request.Id);
            proposta.CriteriosValidacaoInscricao = await repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.Id);
            proposta.VagasRemanecentes = await repositorioProposta.ObterVagasRemacenentesPorId(request.Id);
            proposta.PalavrasChaves = await repositorioProposta.ObterPalavrasChavesPorId(request.Id);
            proposta.Modalidades = await repositorioProposta.ObterModalidadesPorId(request.Id);
            proposta.AnosTurmas = await repositorioProposta.ObterAnosTurmasPorId(request.Id);
            proposta.ComponentesCurriculares = await repositorioProposta.ObterComponentesCurricularesPorId(request.Id);
            proposta.CriterioCertificacao = await repositorioProposta.ObterCriterioCertificacaoPorPropostaId(request.Id);
            proposta.Turmas = await repositorioProposta.ObterTurmasPorId(request.Id);
            proposta.TiposInscricao = await repositorioProposta.ObterTiposInscricaoPorId(request.Id);
            proposta.Movimentacao = await repositorioPropostaMovimentacao.ObterUltimoParecerPropostaId(request.Id, proposta.Situacao);
            proposta.AreaPromotora = await repositorioAreaPromotora.ObterPorId(proposta.AreaPromotoraId);
            proposta.UltimaJustificativaDevolucao = await repositorioPropostaMovimentacao.ObterUltimaJustificativaDevolucao(request.Id);
            proposta.Pareceristas = await repositorioProposta.ObterPareceristasPorId(request.Id);

            foreach (var turma in proposta.Turmas)
                turma.Dres = await repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(turma.Id);

            var propostaCompletaDTO = mapper.Map<PropostaCompletoDTO>(proposta);
            propostaCompletaDTO.Auditoria = mapper.Map<AuditoriaDTO>(proposta);
            propostaCompletaDTO.AreaPromotora = mapper.Map<PropostaAreaPromotoraDTO>(proposta.AreaPromotora);

            var perfilLogado = await mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia(), cancellationToken);
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia(), cancellationToken);
            var consideracoes = await repositorioProposta.ObterPropostaPareceristaConsideracaoPorId(proposta.Id);

            var ehAdminDF = perfilLogado.EhPerfilAdminDF();
            var ehParecerista = perfilLogado.EhPerfilParecerista();
            var parecerista = ehParecerista ? proposta.Pareceristas.FirstOrDefault(a => a.RegistroFuncional.Equals(usuarioLogado.Login)) : null;
            var ehPareceristaDaProposta = parecerista.NaoEhNulo();
            var possuiPareceristasNaProposta = proposta.Pareceristas.Any();
            var possuiPareceristasEnviados = proposta.Pareceristas.Any(a => a.Situacao.EstaEnviada());

            var estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf = proposta.Situacao.EstaAguardandoAnaliseParecerPelaDFOuAreaPromotoraOuAnaliseFinalPelaDF();

            var podeAprovarRecusar = PodeAprovarRecusar(ehParecerista, ehAdminDF, proposta, consideracoes, parecerista);
            var ehAreaPromotora = await EhPerfilAreaPromotora(perfilLogado);
            var totalDeConsideracoes = ObterTotalDePareceresPorCampo(consideracoes, ehAdminDF, proposta.Pareceristas, ehAreaPromotora);

            propostaCompletaDTO.EhParecerista = ehParecerista;
            propostaCompletaDTO.EhAdminDF = ehAdminDF;
            propostaCompletaDTO.EhAreaPromotora = ehAreaPromotora;

            propostaCompletaDTO.PodeEditar = PodeEditar(
                ehAdminDF,
                ehAreaPromotora,
                propostaCompletaDTO.Auditoria.CriadoLogin,
                usuarioLogado.Login
            );

            propostaCompletaDTO.TotalDeConsideracoes = totalDeConsideracoes;
            propostaCompletaDTO.ExibirConsideracoes = PodeExibirParecer(ehAdminDF, possuiPareceristasNaProposta, estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf, ehPareceristaDaProposta, ehAreaPromotora, totalDeConsideracoes.Count());
            propostaCompletaDTO.PodeEnviar = PodeEnviar(proposta, possuiPareceristasNaProposta, ehAdminDF, ehAreaPromotora, possuiPareceristasEnviados);
            propostaCompletaDTO.PodeEnviarConsideracoes = PodeEnviarParecer(ehParecerista, proposta, usuarioLogado, consideracoes);
            propostaCompletaDTO.QtdeLimitePareceristaProposta = await ObterParametroSistema(TipoParametroSistema.QtdeLimitePareceristaProposta);
            propostaCompletaDTO.PodeAprovar = podeAprovarRecusar;
            propostaCompletaDTO.PodeRecusar = podeAprovarRecusar;
            propostaCompletaDTO.LabelAprovar = ehParecerista ? "Sugerir aprovação" : "Aprovar";
            propostaCompletaDTO.LabelRecusar = ehParecerista ? "Sugerir recusa" : "Recusar";

            propostaCompletaDTO.DesativarAnoEhComponente = DesativarAnoEhComponente(proposta);

            propostaCompletaDTO.UltimaJustificativaAprovacaoRecusa = ehPareceristaDaProposta ? parecerista.Justificativa : proposta.Situacao.EstaAprovadaOuRecusada() ? proposta.Movimentacao.Justificativa : string.Empty;

            propostaCompletaDTO.CargaHorariaTotal = proposta.CargaHorariaTotal;
            propostaCompletaDTO.CargaHorariaNaoPresencial = proposta.CargaHorariaNaoPresencial;
            propostaCompletaDTO.OutrosCriterios = proposta.OutrosCriterios;
            propostaCompletaDTO.HorasTotais = proposta.HorasTotais;
            propostaCompletaDTO.CargaHorariaTotalOutra = proposta.CargaHorariaTotalOutra;

            if (proposta.ArquivoImagemDivulgacaoId.HasValue)
            {
                var arquivo = await repositorioArquivo.ObterPorId(proposta.ArquivoImagemDivulgacaoId.Value);
                propostaCompletaDTO.ArquivoImagemDivulgacao = mapper.Map<PropostaImagemDivulgacaoDTO>(arquivo);
            }

            propostaCompletaDTO.GruposPeriodos = await mediator.Send(new ObterPropostaGrupoPeriodoPorPropostaIdQuery(proposta.Id), cancellationToken);

            return propostaCompletaDTO;
        }

        private static bool PodeEditar(
            bool ehAdminDF,
            bool ehAreaPromotora,
            string codigoCriador,
            string codigoUsuarioLogado)
        {
            if (ehAdminDF)
                return true;

            if (!ehAreaPromotora)
                return false;

            bool? ehCriador = codigoCriador?.Equals(codigoUsuarioLogado);

            return ehCriador == true;
        }

        private static bool DesativarAnoEhComponente(Proposta proposta)
        {
            return (proposta.PublicoAlvoOutros.PossuiElementos() || proposta.PublicosAlvo.Any())
                   && (proposta.FuncaoEspecificaOutros.PossuiElementos() || proposta.FuncoesEspecificas.Any());
        }

        private static bool PodeAprovarRecusar(bool ehParecerista, bool ehAdminDF, Proposta proposta, IEnumerable<PropostaPareceristaConsideracao> consideracoes, PropostaParecerista parecerista)
        {
            if (ehParecerista && parecerista.NaoEhNulo())
            {
                if (parecerista.Situacao.EstaAprovadaOuRecusada())
                    return false;

                return (proposta.Situacao.EstaAguardandoAnalisePeloParecerista() && !consideracoes.Any(a => a.PropostaPareceristaId == parecerista.Id))
                       || proposta.Situacao.EstaAguardandoReanalisePeloParecerista();
            }

            return ehAdminDF && (proposta.Situacao.EstaAguardandoAnaliseParecerFinalPelaDF() || proposta.Situacao.EstaAguardandoAnaliseParecerPelaDF());
        }

        private static bool PodeExibirParecer(bool ehAdminDF, bool possuiPareceristasNaProposta, bool estaAguardandoAnaliseParecerPelaDfOuAreaPromotoraOuAnaliseFinalPelaDf,
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
            return (await mediator.Send(new ObterPerfilAreaPromotoraQuery(usuarioLogado))).NaoEhNulo();
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
            var parametro = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(qtdeLimitePareceristaProposta, DateTimeExtension.HorarioBrasilia().Year));
            return int.Parse(parametro.Valor);
        }

        private static bool PodeEnviar(Proposta proposta, bool possuiPareceristasNaProposta, bool ehAdminDF, bool ehAreaPromotora, bool possuiPareceristasEnviados)
        {
            if ((ehAdminDF && (proposta.Situacao.EstaAguardandoAnaliseDf() || (proposta.Situacao.EstaAguardandoAnaliseParecerPelaDF() && possuiPareceristasEnviados)))
                || (ehAreaPromotora && proposta.Situacao.EstaAnaliseParecerPelaAreaPromotora()))
                return possuiPareceristasNaProposta;

            if (ehAreaPromotora && proposta.Situacao.EstaDevolvida())
                return true;

            return proposta.Situacao.EstaCadastrada();
        }


        private static bool PodeEnviarParecer(bool ehParecerista, Proposta proposta, Usuario usuarioLogado, IEnumerable<PropostaPareceristaConsideracao> consideracoes)
        {
            if (ehParecerista)
            {
                var parecerista = proposta.Pareceristas.FirstOrDefault(a => a.RegistroFuncional == usuarioLogado.Login);
                if (parecerista == null)
                    return false;

                return (proposta.Situacao.EstaAguardandoAnalisePeloParecerista() && parecerista.Situacao.EstaAguardandoValidacao() && consideracoes.Any(a => a.PropostaPareceristaId == parecerista.Id));
            }

            return false;
        }
    }
}
