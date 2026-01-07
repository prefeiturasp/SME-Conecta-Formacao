using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class ValidadorCodafListaPresencaService(
        IRepositorioProposta repositorioProposta,
        IRepositorioCodafListaPresenca repositorioLista,
        IRepositorioCodafInscritosListaPresenca repositorioCodafInscritosListaPresenca) : IValidadorCodafListaPresencaService
    {

        public async Task<Erro?> ValidarUnicidadeTurmaListaDePresencaAsync(long propostaTurmaId, long listaPresencaId = 0)
        {
            var jaPossuiLista = await repositorioLista.TurmaJaTemListaDePresencaAsync(propostaTurmaId, listaPresencaId);

            if (jaPossuiLista)
                return Erro.Negocio($"Já existe uma lista de presença cadastrada para esta turma.");

            return null;
        }

        public async Task<Erro?> ValidarVinculoPropostaTurmaAsync(long propostaId, long propostaTurmaId)
        {
            var proposta = await repositorioProposta.ObterPorId(propostaId);
            if (proposta is null)
                return Erro.Validacao("Proposta não encontrada.");

            var turma = await repositorioProposta.ObterTurmaPorId(propostaTurmaId);
            if (turma is null)
                return Erro.Validacao("Turma não encontrada.");

            if (turma.PropostaId != proposta.Id)
                return Erro.Validacao("A turma informada não pertence à formação selecionada.");

            return null;
        }

        public async Task<Erro?> ValidarParaEnvioAoDfAsync(CodafListaPresenca codafListaPresenca)
        {
            var camposObrigatoriosErro = ValidarCamposObrigatoriosCabecalho(codafListaPresenca);
            if (camposObrigatoriosErro is not null)
                return camposObrigatoriosErro;

            var unicidadeTurmaErro = await ValidarUnicidadeTurmaListaDePresencaAsync(codafListaPresenca.PropostaTurmaId, codafListaPresenca.Id);
            if (unicidadeTurmaErro is not null)
                return unicidadeTurmaErro;

            var vinculoPropostaTurmaErro = await ValidarVinculoPropostaTurmaAsync(codafListaPresenca.PropostaId, codafListaPresenca.PropostaTurmaId);
            if (vinculoPropostaTurmaErro is not null)
                return vinculoPropostaTurmaErro;

            var anexosErro = ValidarAnexosAsync(codafListaPresenca);
            if (anexosErro is not null)
                return anexosErro;

            var inscritosErro = await ValidarInscritosNaListaDePresencaAsync(codafListaPresenca);
            if (inscritosErro is not null)
                return inscritosErro;

            return null;
        }

        private static Erro? ValidarCamposObrigatoriosCabecalho(CodafListaPresenca codafListaPresenca)
        {
            if (codafListaPresenca.DataPublicacao is null)
                return Erro.Negocio("A data de publicação da lista de presença é obrigatória.");
            if (codafListaPresenca.NumeroComunicado is null)
                return Erro.Negocio("O número do comunicado DOM é obrigatório.");
            if (codafListaPresenca.PaginaComunicadoDom is null)
                return Erro.Negocio("A página do comunicado DOM é obrigatória.");
            return null;
        }

        private static Erro? ValidarAnexosAsync(CodafListaPresenca codafListaPresenca)
        {
            if (codafListaPresenca.CodafAnexos is null || !codafListaPresenca.CodafAnexos.Any())
                return Erro.Negocio("É obrigatório o envio de ao menos um anexo para a lista de presença.");
            return null;
        }

        private async Task<Erro?> ValidarInscritosNaListaDePresencaAsync(CodafListaPresenca codafListaPresenca)
        {
            var inscritosNaLista = await repositorioCodafInscritosListaPresenca.ObterInscritosPorTurmaAsync(codafListaPresenca.PropostaTurmaId, 1, 1);
            if (inscritosNaLista.TotalRegistros == 0)
                return Erro.Negocio("Não é possível enviar a lista de presença para o DF sem inscritos.");
            if (inscritosNaLista.TotalRegistros != codafListaPresenca.CodafInscricoes.Count)
                return Erro.Negocio($"Há divergência entre a quantidade de inscritos na formação {codafListaPresenca.Proposta?.NomeFormacao} e a lista de presença.");
            if (!codafListaPresenca.CodafInscricoes.All(i => i.PercentualFrequencia is not null && i.PercentualFrequencia >= 0 && i.PercentualFrequencia <= 100))
                return Erro.Negocio("O percentual de frequência de todas as inscrições deve estar entre 0 e 100.");
            if (!codafListaPresenca.CodafInscricoes.All(i => i.AtividadeObrigatorio is not null))
                return Erro.Negocio("O campo 'Atividade Obrigatório' deve ser preenchido para todas as inscrições.");
            if (!codafListaPresenca.CodafInscricoes.All(i => i.ConceitoFinal is not null && i.ConceitoFinal.ToUpper() is "P" or "S" or "NS"))
                return Erro.Negocio("O campo 'Conceito Final' deve ser preenchido corretamente para todas as inscrições.");
            if (!codafListaPresenca.CodafInscricoes.All(i => i.Aprovado is not null))
                return Erro.Negocio("O campo 'Aprovado' deve ser preenchido para todas as inscrições.");

            var todosInscritosDaTurma = await repositorioCodafInscritosListaPresenca.ObterInscritosPorTurmaAsync(codafListaPresenca.PropostaTurmaId, 1, int.MaxValue);

            // Verifica se todos os inscritos da turma estão na lista de presença
            var inscritosFaltantes = todosInscritosDaTurma.Itens
                .Where(t => !codafListaPresenca.CodafInscricoes.Any(i => i.InscricaoId == t.Id))
                .ToList();
            if (inscritosFaltantes.Any())
                return Erro.Negocio($"Há divergência entre a quantidade de inscritos na formação {codafListaPresenca.Proposta?.NomeFormacao} e a lista de presença.");

            return null;
        }
    }
}