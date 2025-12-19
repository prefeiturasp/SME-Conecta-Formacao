using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao
{
    public class SalvarInscricaoImportacaoCommandHandler(IRepositorioInscricao repositorioInscricao, ITransacao transacao) : 
        IRequestHandler<SalvarInscricaoImportacaoCommand, bool>
    {
        public async Task<bool> Handle(SalvarInscricaoImportacaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = request.Inscricao;

            return await PersistirInscricao(inscricao);
        }

        private async Task<bool> PersistirInscricao(Inscricao inscricao)
        {
            var transacaoAtual = transacao.Iniciar();
            try
            {
                await repositorioInscricao.Inserir(inscricao);
                bool confirmada = await repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                if (!confirmada)
                    throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                inscricao.Situacao = SituacaoInscricao.Confirmada;
                await repositorioInscricao.Atualizar(inscricao);

                transacaoAtual.Commit();

                return true;
            }
            catch
            {
                transacaoAtual.Rollback();
                throw;
            }
            finally
            {
                transacaoAtual.Dispose();
            }
        }
    }
}
