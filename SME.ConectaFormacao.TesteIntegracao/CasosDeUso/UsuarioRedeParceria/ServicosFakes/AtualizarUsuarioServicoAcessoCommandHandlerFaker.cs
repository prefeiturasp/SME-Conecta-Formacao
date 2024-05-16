using MediatR;
using SME.ConectaFormacao.Aplicacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes
{
    public class AtualizarUsuarioServicoAcessoCommandHandlerFaker : IRequestHandler<AtualizarUsuarioServicoAcessoCommand, bool>
    {
        public Task<bool> Handle(AtualizarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
