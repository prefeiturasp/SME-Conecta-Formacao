using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaCommandHandler : IRequestHandler<AlterarPropostaCommand, RetornoDTO>
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

        public async Task<RetornoDTO> Handle(AlterarPropostaCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var ehPropostaPublicada = proposta.Situacao.EstaPublicada() || proposta.Situacao.EhAlterando();

            var ehPropostaAutomatica = request.PropostaDTO.TiposInscricao.PossuiElementos() && request.PropostaDTO.TiposInscricao.Any(a => a.TipoInscricao.EhAutomaticaOuJEIF());
            if (request.PropostaDTO.PublicoAlvoOutros.EstaPreenchido() &&
                !request.PropostaDTO.PublicosAlvo.PossuiElementos() &&
                request.PropostaDTO.FuncaoEspecificaOutros.EstaPreenchido() &&
                !request.PropostaDTO.FuncoesEspecificas.PossuiElementos())
            {
                if (!request.PropostaDTO.ComponentesCurriculares.Any() || !request.PropostaDTO.AnosTurmas.Any())
                    erros.Add(MensagemNegocio.INFORMAR_PUBLICO_FUNCAO_MODALIDADE);
            }

            var validarPublicoAlvoOutrosCommand = await _mediator.Send(new ValidarPublicoAlvoOutrosCommand(ehPropostaAutomatica, request.PropostaDTO.PublicosAlvo, request.PropostaDTO.PublicoAlvoOutros), cancellationToken);

            if (validarPublicoAlvoOutrosCommand.Any())
                erros.AddRange(validarPublicoAlvoOutrosCommand);

            var validarFuncaoEspecificaOutrosCommand = await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);
            if (validarFuncaoEspecificaOutrosCommand.Any())
                erros.AddRange(validarFuncaoEspecificaOutrosCommand);

            var validarCriterioValidacaoInscricaoOutrosCommand = await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);
            if (validarCriterioValidacaoInscricaoOutrosCommand.Any())
                erros.AddRange(validarCriterioValidacaoInscricaoOutrosCommand);

            var validarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand = await _mediator.Send(new ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand(request.PropostaDTO.PublicosAlvo, request.PropostaDTO.FuncoesEspecificas,
                request.PropostaDTO.Modalidades, request.PropostaDTO.AnosTurmas, request.PropostaDTO.ComponentesCurriculares), cancellationToken);

            if (validarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand.Any())
                erros.AddRange(validarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand);

            if (erros.Any())
                throw new NegocioException(erros);

            await _mediator.Send(new ValidarResponsavelDfCommand(request.PropostaDTO.RfResponsavelDf,
                request.PropostaDTO.FormacaoHomologada, request.PropostaDTO.Situacao), cancellationToken);

            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);
            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.ManterCriador(proposta);
            propostaDepois.AcaoFormativaTexto = proposta.AcaoFormativaTexto;
            propostaDepois.AcaoFormativaLink = proposta.AcaoFormativaLink;

            await _mediator.Send(new ValidarAreaPromotoraCommand(propostaDepois.AreaPromotoraId, propostaDepois.IntegrarNoSGA), cancellationToken);

            var possuiTurmaSemDrePreenchida = request.PropostaDTO.Turmas.Any(x => x.DresIds.Length == 0);
            if (possuiTurmaSemDrePreenchida)
                erros.Add(MensagemNegocio.DRE_NAO_INFORMADA_PARA_TODAS_AS_TURMAS);

            var dreTodos = await _mediator.Send(ObterDreTodosQuery.Instancia(), cancellationToken);
            var possuiTurmaComTodasAsDres = request.PropostaDTO.Turmas.Any(c => c.DresIds.Contains(dreTodos.Id));
            var possuiTurmaComDreSelecionada = request.PropostaDTO.Turmas.Any(c => !c.DresIds.Contains(dreTodos.Id));
            if (!(possuiTurmaComTodasAsDres || possuiTurmaComDreSelecionada))
                erros.Add(MensagemNegocio.TODAS_AS_TURMAS_DEVEM_POSSUIR_DRE_OU_OPCAO_TODOS);

            var validarDatas = await _mediator.Send(new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(propostaDepois.DataInscricaoFim, propostaDepois.DataRealizacaoFim), cancellationToken);

            if (!string.IsNullOrEmpty(validarDatas))
                erros.Add(validarDatas);

            var errosRegente = await _mediator.Send(new ValidarSeExisteRegenteTurmaCommand(request.Id, propostaDepois.QuantidadeTurmas ?? 0), cancellationToken);
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

                var mensagem = new StringBuilder(string.Format(MensagemNegocio.PROPOSTA_X_ALTERADA_COM_SUCESSO, request.Id));

                if (!ehPropostaPublicada)
                    return RetornoDTO.RetornarSucesso(mensagem.ToString(), request.Id);
                
                if (ehPropostaAutomatica)
                    mensagem.Append(MensagemNegocio.PROPOSTA_PUBLICADA_ALTERADA_COM_INSCRICAO_AUTOMATICA);

                return RetornoDTO.RetornarSucesso(mensagem.ToString(), request.Id);
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