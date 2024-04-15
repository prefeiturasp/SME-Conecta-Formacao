using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarResponsavelDfCommand : IRequest
    {
        public ValidarResponsavelDfCommand(string? rfResponsavelDf, FormacaoHomologada? formacaoHomologada)
        {
            RfResponsavelDf = rfResponsavelDf;
            FormacaoHomologada = formacaoHomologada;
        }

        public string? RfResponsavelDf { get; }
        public FormacaoHomologada? FormacaoHomologada { get; }
    }
}