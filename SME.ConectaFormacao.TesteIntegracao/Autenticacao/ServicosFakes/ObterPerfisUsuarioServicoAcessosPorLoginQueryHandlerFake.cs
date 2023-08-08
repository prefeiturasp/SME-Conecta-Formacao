﻿using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.TesteIntegracao.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Autenticacao.ServicosFakes
{
    internal class ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake : IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>
    {
        public Task<UsuarioPerfisRetornoDTO> Handle(ObterPerfisUsuarioServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == AutenticacaoMock.AutenticacaoUsuarioDTOValido.Login)
            {
                return Task.FromResult(AutenticacaoMock.UsuarioPerfisRetornoDTOValido);
            }

            return Task.FromResult(new UsuarioPerfisRetornoDTO());
        }
    }
}
