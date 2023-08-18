using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoUsuarioSolicitarRecuperacaoSenha : CasoDeUsoAbstrato, ICasoDeUsoUsuarioSolicitarRecuperacaoSenha
    {
        public CasoDeUsoUsuarioSolicitarRecuperacaoSenha(IMediator mediator) : base(mediator)
        {
        }

        public async Task<string> Executar(string login)
        {
            var email = await mediator.Send(new SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand(login));
            if (email.IndexOf('@') < 0)
                throw new NegocioException(MensagemNegocio.LOGIN_NAO_ENCONTRADO);

            return string.Format(MensagemNegocio.ORIENTACOES_RECUPERACAO_SENHA, email.TratarEmail());
        }
    }
}
