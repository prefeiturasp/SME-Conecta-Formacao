using SME.ConectaFormacao.Dominio.Comparadores;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Linq.Expressions;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class UsuarioAcessibilidadeService(IRepositorioUsuarioAcessibilidade repositorioUsuarioAcessibilidade) :
        IUsuarioAcessibilidadeService
    {
        public async Task<long?> SalvarAcessibilidadeDaInscricaoAsync(UsuarioAcessibilidade usuarioAcessibilidade)
        {
            var acessibilidadeAtual = await repositorioUsuarioAcessibilidade.ObterPorUsuarioIdAsync(usuarioAcessibilidade.UsuarioId);

            if (acessibilidadeAtual == usuarioAcessibilidade)
            {
                return acessibilidadeAtual?.Id;
            }

            var dadosSaoIguais = 
                acessibilidadeAtual is not null &&
                UsuarioAcessibilidadeComparadorDados.Instancia.Equals(acessibilidadeAtual, usuarioAcessibilidade);

            if (dadosSaoIguais && usuarioAcessibilidade.Excluido)
            {
                acessibilidadeAtual!.Excluido = usuarioAcessibilidade.Excluido;
                await repositorioUsuarioAcessibilidade.Atualizar(acessibilidadeAtual);
                return acessibilidadeAtual.Id;
            }

            if (acessibilidadeAtual is not null && !acessibilidadeAtual.Excluido)
            {
                acessibilidadeAtual.Excluido = true;
                await repositorioUsuarioAcessibilidade.Atualizar(acessibilidadeAtual);
            }

            return await ObterIdRecicladoOuInserirNovoAsync(usuarioAcessibilidade);
        }

        private async Task<long> ObterIdRecicladoOuInserirNovoAsync(UsuarioAcessibilidade novo)
        {
            var registroExistente = await repositorioUsuarioAcessibilidade.ObterPorExpressaoAsync(MontarPredicadoBusca(novo));

            if (registroExistente is not null)
            {
                if (registroExistente.Excluido != novo.Excluido)
                {
                    registroExistente.Excluido = novo.Excluido;
                    await repositorioUsuarioAcessibilidade.Atualizar(registroExistente);
                }
                return registroExistente.Id;
            }
            return await repositorioUsuarioAcessibilidade.Inserir(novo);
        }

        private static Expression<Func<UsuarioAcessibilidade, bool>> MontarPredicadoBusca(UsuarioAcessibilidade alvo)
        {
            return a => a.UsuarioId == alvo.UsuarioId &&
                        a.PossuiDeficiencia == alvo.PossuiDeficiencia &&
                        a.DescricaoDeficiencia == alvo.DescricaoDeficiencia &&
                        a.NecessitaAdaptacao == alvo.NecessitaAdaptacao &&
                        a.DescricaoAdaptacao == alvo.DescricaoAdaptacao;
        }
    }
}