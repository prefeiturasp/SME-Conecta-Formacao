using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos
{
    public class AlterarNomeSocialServicoAcessosCommandHandler(IServicoAcessos servicoAcessos, IMediator mediator) :
            IRequestHandler<AlterarNomeSocialServicoAcessosCommand, bool>
    {
        public async Task<bool> Handle(AlterarNomeSocialServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return await servicoAcessos.AlterarNomeSocialAsync(request.Login, request.NomeSocial);
        }
    }
}