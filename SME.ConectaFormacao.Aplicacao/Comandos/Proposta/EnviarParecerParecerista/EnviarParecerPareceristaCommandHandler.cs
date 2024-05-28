using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerPareceristaCommandHandler : IRequestHandler<EnviarParecerPareceristaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public EnviarParecerPareceristaCommandHandler(IRepositorioProposta repositorioProposta, IMediator mediator, IMapper mapper)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(EnviarParecerPareceristaCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(request.PropostaId), cancellationToken);
            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var situacoes = new[] { SituacaoProposta.AguardandoAnalisePeloParecerista, SituacaoProposta.AguardandoReanalisePeloParecerista };
            if (!situacoes.Contains(proposta.Situacao))
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECERISTA);

            var parecerista = await _mediator.Send(new ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery(request.PropostaId, request.RegistroFuncional), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.USUARIO_LOGADO_NAO_E_PARECERISTA_DA_PROPOSTA);

            parecerista.Situacao = request.Situacao;

            if (request.Justificativa.PossuiElementos())
                parecerista.Justificativa = request.Justificativa;

            await _repositorioProposta.AtualizarSituacaoParecerista(parecerista.Id, parecerista.RegistroFuncional, parecerista.Situacao, request.Justificativa);

            var pareceristas = await _repositorioProposta.ObterPareceristasPorId(proposta.Id);

            var naoPossuiPreceristasAguardandoValidacao = !pareceristas.Any(a => a.Situacao.EstaAguardandoValidacao());

            var naoPossuiPareceristasAguardandoParecerFinal = !pareceristas.Any(a => a.Situacao.EstaEnviada() || a.Situacao.EstaAguardandoRevalidacao());

            if (proposta.Situacao.EstaAguardandoAnalisePeloParecerista())
            {
                if (naoPossuiPreceristasAguardandoValidacao)
                {
                    await _mediator.Send(new EnviarPropostaCommand(proposta.Id, SituacaoProposta.AguardandoAnaliseParecerPelaDF), cancellationToken);
                    await _mediator.Send(new SalvarPropostaMovimentacaoCommand(proposta.Id, SituacaoProposta.AguardandoAnaliseParecerPelaDF), cancellationToken);
                }
                
                var pareceristaResumido = _mapper.Map<PropostaPareceristaResumidoDTO>(pareceristas.Where(w=> w.RegistroFuncional.Equals(parecerista.RegistroFuncional)));
                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.NotificarDFPeloEnvioParecerPeloParecerista, new NotificacaoPropostaPareceristaDTO(proposta.Id, pareceristaResumido)));
                return true;
            }
            
            if (proposta.Situacao.EstaAguardandoReanalisePeloParecerista())
            {
                if (naoPossuiPareceristasAguardandoParecerFinal)
                {
                    await _mediator.Send(new EnviarPropostaCommand(proposta.Id, SituacaoProposta.AguardandoValidacaoFinalPelaDF), cancellationToken);
                    await _mediator.Send(new SalvarPropostaMovimentacaoCommand(proposta.Id, SituacaoProposta.AguardandoValidacaoFinalPelaDF), cancellationToken);
                }
                
                var pareceristaResumido = _mapper.Map<PropostaPareceristaResumidoDTO>(pareceristas.FirstOrDefault(w=> w.RegistroFuncional.Equals(parecerista.RegistroFuncional)));
                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.NotificarResponsavelDFSobreReanaliseDoParecerista, new NotificacaoPropostaPareceristaDTO(proposta.Id, pareceristaResumido)));
                return true;
            }

            return true;
        }
    }
}
