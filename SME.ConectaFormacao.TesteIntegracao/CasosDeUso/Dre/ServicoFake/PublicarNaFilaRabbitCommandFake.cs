using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.ServicoFake
{
    internal class PublicarNaFilaRabbitCommandFake : IRequestHandler<PublicarNaFilaRabbitCommand, bool>
    {
        public async Task<bool> Handle(PublicarNaFilaRabbitCommand request, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
