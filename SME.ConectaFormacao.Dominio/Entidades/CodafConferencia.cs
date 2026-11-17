using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafConferencia : EntidadeBaseAuditavel
    {
        public long PropostaId { get; private set; }
        public long PropostaTurmaId { get; private set; }
        public short? NumeroComunicado { get; private set; }
        public DateTime? DataComunicado { get; private set; }
        public short? PaginaComunicadoDom { get; private set; }
        public DateTime? DataPublicacaoDom { get; private set; }
        public int? CodigoCursoEol { get; private set; }
        public int? CodigoNivel { get; private set; }
        public string? Observacao { get; private set; }
        public StatusCodafConferencia Status { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }
        public bool CertificadoEmitido => CodafCertificados is not null && CodafCertificados.Count > 0;

        public Proposta Proposta { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
        public ICollection<CodafConferenciaInscricao> CodafInscricoes { get; set; } = [];
        public ICollection<CodafConferenciaAnexo>? CodafAnexos { get; set; }
        public ICollection<CodafCertificado>? CodafCertificados { get; set; }

        public CodafConferencia() { }

        public CodafConferencia(long propostaId, long propostaTurmaId)
        {
            PropostaId = propostaId;
            PropostaTurmaId = propostaTurmaId;
            Status = StatusCodafConferencia.Iniciado;
        }

        public CodafConferencia(long propostaId, long propostaTurmaId, DadosPublicacaoLista dadosPublicacao)
        {
            PropostaId = propostaId;
            PropostaTurmaId = propostaTurmaId;
            Status = StatusCodafConferencia.Iniciado;
            AtribuirDadosPublicacao(dadosPublicacao);
        }

        public void AtualizarInformacoes(DadosPublicacaoLista dadosPublicacao)
        {
            AtribuirDadosPublicacao(dadosPublicacao);
        }

        private void AtribuirDadosPublicacao(DadosPublicacaoLista dados)
        {
            DataComunicado = dados.DataPublicacao;
            DataPublicacaoDom = dados.DataPublicacaoDom;
            NumeroComunicado = dados.NumeroComunicado;
            PaginaComunicadoDom = dados.PaginaComunicadoDom;
            CodigoCursoEol = dados.CodigoCursoEol;
            CodigoNivel = dados.CodigoNivel;
            Observacao = dados.Observacao;
        }

        public void Iniciar()
        {
            Status = StatusCodafConferencia.Iniciado;
        }

        public void Finalizar()
        {
            if (Status == StatusCodafConferencia.Aguardando)
            {
                Status = StatusCodafConferencia.Finalizado;
                DataFinalizacao = DateTime.UtcNow;
            }
        }

        public bool EstaFinalizado()
            => Status == StatusCodafConferencia.Finalizado;

        public bool PodeSerExcluido(Guid? perfilId = null)
            => Status == StatusCodafConferencia.Iniciado;

        public void DefinirStatus()
        {
            if (Status == StatusCodafConferencia.Finalizado)
                return;

            if (PropostaId > 0 &&
                PropostaTurmaId > 0 &&
                CodigoCursoEol.HasValue &&
                CodafInscricoes is not null &&
                CodafInscricoes.Count != 0 &&
                CodafInscricoes.All(i => i.Participou.HasValue))
            {
                Status = StatusCodafConferencia.Aguardando;
                return;
            }

            Status = StatusCodafConferencia.Iniciado;
        }
    }
}
