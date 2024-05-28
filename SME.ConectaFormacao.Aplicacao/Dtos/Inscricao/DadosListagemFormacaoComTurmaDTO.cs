using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemFormacaoComTurmaDTO
    {
        public long Id { get; set; }
        public long CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public IEnumerable<DadosListagemFormacaoTurma> Turmas { get; set; }
        public IEnumerable<TipoInscricao> TiposInscricoes { get; set; }
        public DadosListagemInscricaoPermissaoDTO Permissao { get; set; }
        public class DadosListagemInscricaoPermissaoDTO
        {
            public bool PodeCancelar { get; set; }
            public bool PodeColocarEmEspera { get; set; }
            public bool PodeConfirmar { get; set; }
        }
    }
}