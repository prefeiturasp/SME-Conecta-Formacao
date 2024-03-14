using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.Inscricao
{
    public class ArquivosImportadosDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public SituacaoImportacaoArquivo Situacao { get; set; }
        public long TotalRegistros { get; set; }
        public long TotalProcessados { get; set; }
    }
}
