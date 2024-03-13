using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterNomeCursistaInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterNomeCursistaInscricao
    {
        public CasoDeUsoObterNomeCursistaInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<string> Executar(string? registroFuncional, string? cpf)
        {
            var login = registroFuncional.EstaPreenchido() ? registroFuncional : cpf;

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()));
            if (usuario.NaoEhNulo())
                return usuario.Nome;

            if (registroFuncional.EstaPreenchido())
            {
                var nomeEol = await mediator.Send(new ObterNomeProfissionalPorRegistroFuncionalQuery(registroFuncional));

                if (nomeEol.EstaPreenchido())
                    return nomeEol;
            }

            throw new NegocioException(MensagemNegocio.CURSISTA_NAO_ENCONTRADO);
        }
    }
}
