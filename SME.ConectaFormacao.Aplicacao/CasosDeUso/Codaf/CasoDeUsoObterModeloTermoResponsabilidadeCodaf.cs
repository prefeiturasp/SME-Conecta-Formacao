using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using System.Reflection;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoObterModeloTermoResponsabilidadeCodaf : ICasoDeUsoObterModeloTermoResponsabilidadeCodaf
    {
        private const string CAMINHO_RECURSO = "SME.ConectaFormacao.Aplicacao.Assets.TermoResponsabilidadeModelo.pdf";
        private const string NOME_ARQUIVO_DOWNLOAD = "TermoResponsabilidadeModelo.pdf";
        private const string CONTENT_TYPE = "application/pdf";

        private readonly Assembly _assembly;

        public CasoDeUsoObterModeloTermoResponsabilidadeCodaf(Assembly? assembly = null)
        {
            _assembly = assembly ?? Assembly.GetExecutingAssembly();
        }

        public Resultado<ArquivoDto> Executar()
        {
            var stream = _assembly.GetManifestResourceStream(CAMINHO_RECURSO);

            if (stream == null)
                return Erro.NaoEncontrado("Não foi possível localizar o modelo do termo de responsabilidade.");

            return new ArquivoDto(NOME_ARQUIVO_DOWNLOAD, CONTENT_TYPE, stream);
        }
    }
}
