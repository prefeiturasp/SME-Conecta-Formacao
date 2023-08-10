﻿using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.TesteIntegracao.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Autenticacao.ServicosFakes
{
    internal class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>
    {
        public Task<UsuarioAutenticacaoRetornoDTO> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == AutenticacaoMock.AutenticacaoUsuarioDTOValido.Login && AutenticacaoMock.AutenticacaoUsuarioDTOValido.Senha == request.Senha)
            {
                return Task.FromResult(AutenticacaoMock.UsuarioAutenticacaoRetornoDTOValido);
            }

            return Task.FromResult(new UsuarioAutenticacaoRetornoDTO());
        }
    }
}
