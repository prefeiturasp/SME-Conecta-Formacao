using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorUnidadeEol;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoObterUsuariosPorEolUnidade(IMediator mediator) : ICasoDeUsoObterUsuariosPorEolUnidade
    {
        public async Task<IEnumerable<DadosLoginUsuarioDto>> ExecutarAsync(string codigoEolUnidade, string? login, string? nome) =>
            await mediator.Send(new ObterUsuariosPorEolUnidadeQuery(codigoEolUnidade, login, nome));
    }
}
