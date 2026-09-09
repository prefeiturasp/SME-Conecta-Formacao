using SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public class UtilitariosPerfis : IUtilitariosPerfis
    {
        private readonly IContextoAplicacao contextoAplicacao;

        public UtilitariosPerfis(IContextoAplicacao contextoAplicacao)
        {
            this.contextoAplicacao = contextoAplicacao;
        }

        public bool FiltrarPorPerfil()
        {
            var perfilCursista = PerfilAutomatico.PERIL_CURSISTA_CODIGO;
            if (contextoAplicacao.IdPerfilUsuario != perfilCursista) return false;
            if (string.IsNullOrWhiteSpace(contextoAplicacao.LoginUsuario)) return false;
            if (UtilValidacoes.CpfEhValido(contextoAplicacao.LoginUsuario)) return false;
            return true;
        }
    }
}
