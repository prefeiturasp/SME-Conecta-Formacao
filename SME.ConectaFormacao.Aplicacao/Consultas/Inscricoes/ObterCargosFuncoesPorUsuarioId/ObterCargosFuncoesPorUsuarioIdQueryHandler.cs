using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes.ObterCargosFuncoesPorUsuarioId;

public class ObterCargosFuncoesPorUsuarioIdQueryHandler(IRepositorioInscricao repositorioInscricao)
    : IRequestHandler<ObterCargosFuncoesPorUsuarioIdQuery, IEnumerable<DadosInscricaoCargoEol>>
{
    public async Task<IEnumerable<DadosInscricaoCargoEol>> Handle(
        ObterCargosFuncoesPorUsuarioIdQuery request, CancellationToken cancellationToken)
    {
        var cargos = await repositorioInscricao.ObterCargosFuncoesPorUsuarioId(request.UsuarioId);

        return cargos.Select(c => new DadosInscricaoCargoEol
        {
            Codigo = c.Codigo,
            Descricao = c.Descricao,
            UeCodigo = c.UeCodigo,
            DreCodigo = c.DreCodigo,
            TipoVinculo = c.TipoVinculo,
            DataInicio = c.DataInicio
        });
    }
}
