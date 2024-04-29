﻿using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerPareceristaCommandHandler : IRequestHandler<EnviarParecerPareceristaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public EnviarParecerPareceristaCommandHandler(IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(EnviarParecerPareceristaCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _mediator.Send(new ObterUsuarioLogadoQuery());

            if (!(await _repositorioProposta.ExistePareceristasPendenteDeEnvio(request.IdProposta, usuario.Id)))
            {
                await _mediator.Send(new EnviarPropostaCommand(request.IdProposta, SituacaoProposta.AguardandoAnaliseDf));
                await _mediator.Send(new SalvarPropostaMovimentacaoCommand(request.IdProposta, SituacaoProposta.AguardandoAnaliseDf));
            }

            await _repositorioProposta.AtualizarSituacaoDoParecerEnviadaPeloParecerista(request.IdProposta, usuario.Id);

            return true;
        }
    }
}