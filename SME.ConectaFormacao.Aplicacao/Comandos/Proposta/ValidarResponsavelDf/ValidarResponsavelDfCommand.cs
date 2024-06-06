using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarResponsavelDfCommand : IRequest
    {
        public ValidarResponsavelDfCommand(string? rfResponsavelDf, FormacaoHomologada? formacaoHomologada,
            SituacaoProposta situacaoProposta)
        {
            RfResponsavelDf = rfResponsavelDf;
            FormacaoHomologada = formacaoHomologada;
            SituacaoProposta = situacaoProposta;
        }

        public string? RfResponsavelDf { get; }
        public FormacaoHomologada? FormacaoHomologada { get; }
        public SituacaoProposta SituacaoProposta { get; }
    }
}