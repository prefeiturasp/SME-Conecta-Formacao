using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomesFuncionarioPorRf
{
    public class ObterNomesFuncionarioPorRfQuery(string rf) : IRequest<FuncionarioNomesDto?>
    {
        public string Rf { get; set; } = rf;
    }
}