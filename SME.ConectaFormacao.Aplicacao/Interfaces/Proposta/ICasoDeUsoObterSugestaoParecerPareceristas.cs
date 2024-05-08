using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterSugestaoParecerPareceristas
    {
        Task<string> Executar(long propostaId, SituacaoParecerista situacaoParecerista);
    }
}
