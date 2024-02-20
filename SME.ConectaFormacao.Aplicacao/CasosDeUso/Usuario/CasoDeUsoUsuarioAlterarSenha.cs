using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioAlterarSenha : CasoDeUsoAbstrato, ICasoDeUsoUsuarioAlterarSenha
    {
        public CasoDeUsoUsuarioAlterarSenha(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(string login, AlterarSenhaUsuarioDTO alterarSenhaUsuarioDto)
        {
            if (alterarSenhaUsuarioDto.SenhaNova != alterarSenhaUsuarioDto.ConfirmarSenha)
                throw new NegocioException(MensagemNegocio.CONFIRMACAO_SENHA_INVALIDA);

            if (!SenhaAtendeCriteriosDeSeguranca(alterarSenhaUsuarioDto.SenhaNova))
                throw new NegocioException(MensagemNegocio.SENHA_NAO_ATENDE_CRITERIOS_SEGURANCA);

            var sucesso = await mediator.Send(new AlterarSenhaServicoAcessosCommand(login, alterarSenhaUsuarioDto.SenhaAtual, alterarSenhaUsuarioDto.SenhaNova));
            if (!sucesso)
                throw new NegocioException(MensagemNegocio.LOGIN_OU_SENHA_ATUAL_NAO_CONFEREM);

            return sucesso;
        }

        private static bool SenhaAtendeCriteriosDeSeguranca(string senha)
        {
            string regexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d|\w)(?!.*[áàâãéèêíïóôõöúçñ])(?=.*[a-zA-Z\d\w]).{8,12}$";
            Regex regex = new(regexPattern);

            return regex.IsMatch(senha);
        }
    }
}
