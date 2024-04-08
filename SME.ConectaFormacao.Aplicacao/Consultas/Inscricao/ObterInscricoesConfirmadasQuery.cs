using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricoesConfirmadasQuery : IRequest<IEnumerable<Inscricao>>
    {

    }
}