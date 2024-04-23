using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas
{
    public class ObterUsuariosPareceristasQuery : IRequest<IEnumerable<RetornoUsuriosPareceristasDTO>>
    {
        public string? Rf { get; set; }

        public ObterUsuariosPareceristasQuery(string? rf, string? nome)
        {
            Rf = rf;
            Nome = nome;
        }

        public string? Nome { get; set; }
    }
}