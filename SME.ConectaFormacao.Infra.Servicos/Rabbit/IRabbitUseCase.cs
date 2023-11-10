using SME.ConectaFormacao.Dominio;

namespace SME.ConectaFormacao.Infra
{
    public interface IRabbitUseCase : IUseCase<MensagemRabbit, bool>
    {
    }
}
