using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaCommandHandler : IRequestHandler<AlterarPropostaCommand, long>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;

        public AlterarPropostaCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long> Handle(AlterarPropostaCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);

            await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);

            await _mediator.Send(new ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand(request.PropostaDTO.PublicosAlvo, request.PropostaDTO.FuncoesEspecificas,
                request.PropostaDTO.Modalidades, request.PropostaDTO.AnosTurmas, request.PropostaDTO.ComponentesCurriculares), cancellationToken);

            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);
            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.ManterCriador(proposta);
            propostaDepois.AcaoFormativaTexto = proposta.AcaoFormativaTexto;
            propostaDepois.AcaoFormativaLink = proposta.AcaoFormativaLink;

            await _mediator.Send(new ValidarAreaPromotoraCommand(propostaDepois.AreaPromotoraId, propostaDepois.IntegrarNoSGA), cancellationToken);

            var erros = new List<string>();

            var possuiTurmaSemDrePreenchida = request.PropostaDTO.Turmas.Any(x => x.DresIds.Length == 0);
            if (possuiTurmaSemDrePreenchida)
                erros.Add(MensagemNegocio.DRE_NAO_INFORMADA_PARA_TODAS_AS_TURMAS);

            if (!possuiTurmaSemDrePreenchida)
            {
                var dreTodos = await _mediator.Send(ObterDreTodosQuery.Instancia(), cancellationToken);
                var possuiTurmaComTodasAsDres = request.PropostaDTO.Turmas.Any(c => c.DresIds.Contains(dreTodos.Id));
                var possuiTurmaComDreSelecionada = request.PropostaDTO.Turmas.Any(c => !c.DresIds.Contains(dreTodos.Id));

                if (possuiTurmaComTodasAsDres && possuiTurmaComDreSelecionada)
                    erros.Add(MensagemNegocio.TODAS_AS_TURMAS_DEVEM_POSSUIR_DRE_OU_OPCAO_TODOS);
            }

            var validarDatas = await _mediator.Send(new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(proposta.DataInscricaoFim, proposta.DataRealizacaoFim), cancellationToken);

            if (!string.IsNullOrEmpty(validarDatas))
                erros.Add(validarDatas);

            var errosRegente = await _mediator.Send(new ValidarSeExisteRegenteTutorCommand(request.Id, propostaDepois.QuantidadeTurmas ?? 0), cancellationToken);
            if (!string.IsNullOrEmpty(errosRegente))
                erros.Add(errosRegente);

            var errosInformacoesGerais = await _mediator.Send(new ValidarInformacoesGeraisCommand(request.PropostaDTO), cancellationToken);
            if (errosInformacoesGerais.Any())
                erros.AddRange(errosInformacoesGerais);

            var errosDatas = await _mediator.Send(new ValidarDatasExistentesNaPropostaCommand(request.Id, request.PropostaDTO), cancellationToken);
            if (errosDatas.Any())
                erros.AddRange(errosDatas);

            var errosDetalhamento = await _mediator.Send(new ValidarDetalhamentoDaPropostaCommand(request.PropostaDTO), cancellationToken);
            if (errosDetalhamento.Any())
                erros.AddRange(errosDetalhamento);

            var errosCritériosCertificacao = await _mediator.Send(new ValidarCertificacaoPropostaCommand(request.PropostaDTO), cancellationToken);
            if (errosCritériosCertificacao.Any())
                erros.AddRange(errosCritériosCertificacao);

            if (erros.Any())
                throw new NegocioException(erros);

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioProposta.Atualizar(propostaDepois);

                await _mediator.Send(new SalvarPropostaCommand(propostaDepois.Id, propostaDepois, proposta.ArquivoImagemDivulgacaoId), cancellationToken);
                transacao.Commit();
                return request.Id;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}