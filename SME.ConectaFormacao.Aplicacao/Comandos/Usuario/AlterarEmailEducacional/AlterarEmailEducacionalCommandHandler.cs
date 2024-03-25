using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Components;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Usuario.AlterarEmailEducacional
{
    public class AlterarEmailEducacionalCommandHandler : IRequestHandler<AlterarEmailEducacionalCommand,bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public AlterarEmailEducacionalCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(AlterarEmailEducacionalCommand request, CancellationToken cancellationToken)
        {
            var pattern = @"@edu\.sme\.prefeitura\.sp\.gov\.br$";
            if (!Regex.IsMatch(request.Email, pattern, RegexOptions.IgnoreCase))
                throw new NegocioException(MensagemNegocio.EMAIL_EDU_INVALIDO);

            var realizouAtualizacao = await _repositorioUsuario.AtualizarEmailEducacional(request.Login, request.Email);

            if (!realizouAtualizacao)
                throw new NegocioException(MensagemNegocio.EMAIL_NAO_ATUALIZADO);
            
            return realizouAtualizacao;
        }
    }
}