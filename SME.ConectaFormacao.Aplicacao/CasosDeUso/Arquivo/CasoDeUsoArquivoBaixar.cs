using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo
{
    public class CasoDeUsoArquivoBaixar : CasoDeUsoAbstrato, ICasoDeUsoArquivoBaixar
    {
        public CasoDeUsoArquivoBaixar(IMediator mediator) : base(mediator)
        {
        }

        public Task<ArquivoBaixadoDTO> Executar(Guid codigoArquivo)
        {
            return mediator.Send(new ObterArquivoBaixarQuery(codigoArquivo));
        }
    }
}
