using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafListaPresenca : EntidadeBaseAuditavel
    {
        public long PropostaId { get; private set; }
        public long PropostaTurmaId { get; private set; }
        public DateTime? DataPublicacao { get; private set; }
        public DateTime? DataPublicacaoDom { get; private set; }
        public short? NumeroComunicado { get; private set; }
        public short? PaginaComunicadoDom { get; private set; }
        public int? CodigoCursoEol { get; private set; }
        public int? CodigoNivel { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafListaPresenca Status { get; private set; }

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
        public ICollection<CodafComentarioListaPresenca> CodafComentarios { get; set; } = [];
        public ICollection<CodafInscricaoListaPresenca> CodafInscricoes { get; set; } = [];
        public ICollection<CodafRetificacaoListaPresenca> CodafRetificacoes { get; set; } = [];
        public ICollection<CodafAnexo>? CodafAnexos { get; set; }

        protected CodafListaPresenca() { }

        public CodafListaPresenca(long propostaId, long propostaTurmaId, DadosPublicacaoLista dadosPublicacao, Guid? idPerfilUsuario)
        {
            PropostaId = propostaId;
            PropostaTurmaId = propostaTurmaId; 
            AtribuirDadosPublicacao(dadosPublicacao, idPerfilUsuario);
        }
        public void AtualizarInformacoes(DadosPublicacaoLista dadosPublicacao, Guid? idPerfilUsuario)
        {
            AtribuirDadosPublicacao(dadosPublicacao, idPerfilUsuario);
        }
        private void AtribuirDadosPublicacao(DadosPublicacaoLista dados, Guid? idPerfilUsuario)
        {
            DataPublicacao = dados.DataPublicacao;
            DataPublicacaoDom = dados.DataPublicacaoDom;
            NumeroComunicado = dados.NumeroComunicado;
            PaginaComunicadoDom = dados.PaginaComunicadoDom;
            Observacao = dados.Observacao;

            if (idPerfilUsuario == Perfis.ADMIN_DF)
            {
                CodigoNivel = dados.CodigoNivel;
                CodigoCursoEol = dados.CodigoCursoEol;
            }
        }

        public void Iniciar()
        {
            Status = StatusCodafListaPresenca.Iniciado;
        }

        public void MarcarComoEnviadaParaDf()
        {
            if (PodeSerEnviadaParaDf())
                Status = StatusCodafListaPresenca.AguardandoDf;
        }

        public bool PodeSerEnviadaParaDf()
        {
            return Status == StatusCodafListaPresenca.Iniciado || Status == StatusCodafListaPresenca.DevolvidoParaCorrecao;
        }

        public void MarcarComoDevolvidaParaCorrecao()
        {
            if (PodeSerDevolvidaParaCorrecao())
                Status = StatusCodafListaPresenca.DevolvidoParaCorrecao;
        }

        public bool PodeSerDevolvidaParaCorrecao()
        {
            return Status == StatusCodafListaPresenca.AguardandoDf;
        }

        public bool PodeSerExcluido(Guid? idPerfilUsuario)
        {
            if (idPerfilUsuario == Perfis.ADMIN_DF)
                return Status != StatusCodafListaPresenca.Finalizado;
            return Status == StatusCodafListaPresenca.Iniciado;
        }

        public void Finalizar()
        {
            if (Status == StatusCodafListaPresenca.AguardandoDf)
                Status = StatusCodafListaPresenca.Finalizado;
        }
        public bool EstaFinalizado()
            => Status == StatusCodafListaPresenca.Finalizado;
    }
}