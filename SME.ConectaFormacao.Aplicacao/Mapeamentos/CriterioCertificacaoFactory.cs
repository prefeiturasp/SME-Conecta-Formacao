using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public static class CriterioCertificacaoFactory
    {
        public static RegrasAprovacaoCursistaDto ConstruirRegras(IEnumerable<long> criteriosIds)
        {
            var regras = new RegrasAprovacaoCursistaDto();
            var conceitosAceitos = new List<string>();

            if (criteriosIds.Contains((int)TipoCriterioCertificacao.ConceitoParticipacao))
            {
                conceitosAceitos.Add("P");
                conceitosAceitos.Add("S");
            }

            regras.ConceitosAceitos = conceitosAceitos;

            // Regras de frequência (pega a mais restritiva caso venha sujo do banco)
            if (criteriosIds.Contains((int)TipoCriterioCertificacao.FrequenciaIntegral))
                regras.FrequenciaMinima = 100;
            else if (criteriosIds.Contains((int)TipoCriterioCertificacao.FrequenciaMinima75))
                regras.FrequenciaMinima = 75;

            regras.ExigeAtividadeObrigatoria = criteriosIds.Contains((int)TipoCriterioCertificacao.AtividadeObrigatoria);

            return regras;
        }
    }
}
