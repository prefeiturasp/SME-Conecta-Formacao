using MediatR;
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

            var pareceristas = await _repositorioProposta.ObterPareceristasPorPropostaId(request.IdProposta);
            
            if (!pareceristas.Any(a=> !a.RegistroFuncional.Equals(usuario.Login) && a.Situacao.EstaAguardandoValidacao()))
            {
                await _mediator.Send(new EnviarPropostaCommand(request.IdProposta, SituacaoProposta.AguardandoAnaliseParecerPelaDF));
                await _mediator.Send(new SalvarPropostaMovimentacaoCommand(request.IdProposta, SituacaoProposta.AguardandoAnaliseParecerPelaDF));
            }

            await _repositorioProposta.AtualizarSituacaoDoPareceristaParaEnviada(pareceristas.FirstOrDefault(a=> a.RegistroFuncional.Equals(usuario.Login)).Id);

            return true;
        }
    }
}
