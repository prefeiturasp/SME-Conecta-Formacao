using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    public static class MensagemQueueSpy
    {
        public static List<object> MensagensEnviadas { get; } = [];
        public static void Limpar() => MensagensEnviadas.Clear();
    }

    public class PublicarNaFilaRabbitCommandFake : IRequestHandler<PublicarNaFilaRabbitCommand, bool>
    {
        public async Task<bool> Handle(PublicarNaFilaRabbitCommand request, CancellationToken cancellationToken)
        {
            if (request is not null)
                MensagemQueueSpy.MensagensEnviadas.Add(request);

            await Task.Yield();
            return true;
        }
    }
}
