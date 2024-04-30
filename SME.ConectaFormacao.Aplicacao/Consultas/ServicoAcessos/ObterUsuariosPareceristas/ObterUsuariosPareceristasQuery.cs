using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas
{
    public class ObterUsuariosPareceristasQuery : IRequest<IEnumerable<RetornoUsuarioLoginNomeDTO>>
    {
        
    }
}