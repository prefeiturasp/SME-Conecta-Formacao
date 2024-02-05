using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno
{
    public class ObterDadosFuncionarioExternoQueryHandler : IRequestHandler<ObterDadosFuncionarioExternoQuery,IEnumerable<FuncionarioExternoServicoEol>>
    {
        private readonly IServicoEol _servicoEol;

        public ObterDadosFuncionarioExternoQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<IEnumerable<FuncionarioExternoServicoEol>> Handle(ObterDadosFuncionarioExternoQuery request, CancellationToken cancellationToken)
        {
            return await _servicoEol.ObterDadosFuncionarioExternoPorCpf(request.Cpf);
        }
    }
}