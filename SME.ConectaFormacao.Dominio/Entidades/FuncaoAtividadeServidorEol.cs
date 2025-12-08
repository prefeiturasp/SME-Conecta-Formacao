namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class FuncaoAtividadeServidorEol
    {
        public Guid Id { get; private set; }
        public string CdRegistroFuncional { get; private set; }
        public int CdTipoFuncao { get; private set; }
        public string CdDre { get; private set; }
        public string CdUe { get; private set; }
        public DateTime DataAtualizacao { get; private set; }
        public DateOnly? DataPosse { get; set; }
        public string? NomeFuncao { get; set; }
        public int? TipoVinculo { get; set; }

        public FuncaoAtividadeServidorEol(string cdRegistroFuncional, int cdTipoFuncao, string cdDre, string cdUe)
        {
            Id = Guid.NewGuid();
            CdRegistroFuncional = cdRegistroFuncional;
            CdTipoFuncao = cdTipoFuncao;
            CdDre = cdDre;
            CdUe = cdUe;
            DataAtualizacao = DateTime.UtcNow;
        }

        protected FuncaoAtividadeServidorEol()  // EF Core
        {
            CdRegistroFuncional = null!;
            CdDre = null!;
            CdUe = null!;
        }
    }
}
