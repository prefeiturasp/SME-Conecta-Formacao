using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Usuario.UsuarioPossuiProposta
{
    public class UsuarioPossuiPropostaQuery : IRequest<bool>
    {
        public UsuarioPossuiPropostaQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }
}
