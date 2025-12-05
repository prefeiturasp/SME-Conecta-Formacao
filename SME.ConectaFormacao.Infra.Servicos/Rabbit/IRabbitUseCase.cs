using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Infra
{
    public interface IRabbitUseCase : IUseCase<MensagemRabbit, bool>
    {
    }
}
