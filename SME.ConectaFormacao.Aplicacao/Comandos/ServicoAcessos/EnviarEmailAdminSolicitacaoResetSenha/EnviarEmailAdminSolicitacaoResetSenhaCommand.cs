using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.EnviarEmailAdminSolicitacaoResetSenha
{
    public class EnviarEmailAdminSolicitacaoResetSenhaCommand : IRequest<bool>
    {
        public string Login { get; set; }

        public EnviarEmailAdminSolicitacaoResetSenhaCommand(string login)
        {
            Login = login;
        }
    }
}
