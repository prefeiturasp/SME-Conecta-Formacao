using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario
{
    public class CasoDeUsoObterParecerista : CasoDeUsoAbstrato, ICasoDeUsoObterParecerista
    {
        public CasoDeUsoObterParecerista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<UsuarioPareceristaDto>> Executar(string nome, string rf)
        {
            var consulta = await mediator.Send(new ObterUsuariosPareceristasQuery(rf,nome));

            if (!consulta.Any())
                return Enumerable.Empty<UsuarioPareceristaDto>();

            return consulta.Select(x => new UsuarioPareceristaDto() { Nome = x.Nome, Rf = x.Login});
        }
    }
}