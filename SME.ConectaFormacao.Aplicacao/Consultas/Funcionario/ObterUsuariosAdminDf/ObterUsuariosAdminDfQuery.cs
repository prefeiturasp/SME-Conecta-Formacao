﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosAdminDfQuery : IRequest<IEnumerable<RetornoUsuarioLoginNomeDTO>>
    {
    }
}