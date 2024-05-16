using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoInserirUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoInserirUsuarioRedeParceria
    {
        public CasoDeUsoInserirUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            ValidarPreenchimento(usuarioRedeParceriaDTO);

            usuarioRedeParceriaDTO.Cpf = usuarioRedeParceriaDTO.Cpf.SomenteNumeros();
            usuarioRedeParceriaDTO.Telefone = usuarioRedeParceriaDTO.Telefone.SomenteNumeros();

            var usuario = new Dominio.Entidades.Usuario(
                usuarioRedeParceriaDTO.Cpf,
                usuarioRedeParceriaDTO.Nome,
                usuarioRedeParceriaDTO.Email)
            {
                Tipo = Dominio.Enumerados.TipoUsuario.RedeParceria,
                AreaPromotoraId = usuarioRedeParceriaDTO.AreaPromotoraId,
                Telefone = usuarioRedeParceriaDTO.Telefone
            };

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            // TODO: Criar usuário no CORESSO.

            return true;
        }

        private static void ValidarPreenchimento(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            var erros = new List<string>();
            if (!UtilValidacoes.CpfEhValido(usuarioRedeParceriaDTO.Cpf))
                erros.Add(MensagemNegocio.CPF_COM_DIGITO_VERIFICADOR_INVALIDO.Parametros(usuarioRedeParceriaDTO.Cpf));
            if (!UtilValidacoes.NomeComSobrenome(usuarioRedeParceriaDTO.Nome))
                erros.Add(MensagemNegocio.NOME_DEVE_TER_SOBRENOME);
            if (!UtilValidacoes.EmailEhValido(usuarioRedeParceriaDTO.Email))
                erros.Add(MensagemNegocio.EMAIL_INVALIDO);

            if (erros.PossuiElementos())
                throw new NegocioException(erros);
        }
    }
}
