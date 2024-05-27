using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoConfirmarInscricoes : CasoDeUsoAbstrato, ICasoDeUsoConfirmarInscricoes
    {
        public CasoDeUsoConfirmarInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long[] ids)
        {
            var erros = new List<string>();
            foreach (long id in ids.OrderBy(o => o))
            {
                try
                {
                    await mediator.Send(new ConfirmarInscricaoCommand(id));
                }
                catch(NegocioException ex)
                {
                    erros.AddRange(ex.Mensagens);
                }
            }

            if (erros.Any())
                return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_NAO_CONFIRMADAS_POR_FALTA_DE_VAGA);

            return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_CONFIRMADAS_COM_SUCESSO);
        }
    }
}
