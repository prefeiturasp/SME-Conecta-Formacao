using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoAlterarUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoAlterarUsuarioRedeParceria
    {
        public CasoDeUsoAlterarUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id, UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            ValidarPreenchimento(usuarioRedeParceriaDTO);

            usuarioRedeParceriaDTO.Cpf = usuarioRedeParceriaDTO.Cpf.SomenteNumeros();

            var usuario = await mediator.Send(new ObterUsuarioPorIdQuery(id)) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            usuario.Nome = usuarioRedeParceriaDTO.Nome;
            usuario.Email = usuarioRedeParceriaDTO.Email;
            usuario.Telefone = usuarioRedeParceriaDTO.Telefone;
            usuario.AreaPromotoraId = usuarioRedeParceriaDTO.AreaPromotoraId;

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            // TODO: Atualizar usuário no CORESSO.

            return true;
        }

        private static void ValidarPreenchimento(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            if (!UtilValidacoes.NomeComSobrenome(usuarioRedeParceriaDTO.Nome))
                throw new NegocioException(MensagemNegocio.NOME_DEVE_TER_SOBRENOME);
            if (!UtilValidacoes.EmailEhValido(usuarioRedeParceriaDTO.Email))
                throw new NegocioException(MensagemNegocio.EMAIL_INVALIDO);
        }
    }
}
