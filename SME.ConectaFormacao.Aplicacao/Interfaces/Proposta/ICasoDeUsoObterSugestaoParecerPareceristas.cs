using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterSugestaoParecerPareceristas
    {
        Task<IEnumerable<PropostaPareceristaSugestaoDTO>> Executar(long propostaId);
    }
}
