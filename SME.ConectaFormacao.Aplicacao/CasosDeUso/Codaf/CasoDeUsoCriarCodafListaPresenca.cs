using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoCriarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioProposta repositorioProposta,
        IContextoAplicacao contextoAplicacao,
        IMapper mapper) :
        ICasoDeUsoCriarCodafListaPresenca
    {
        public async Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto)
        {
            var erroValidacao = await ValidarRegrasDeNegocio(codafListaPresencaCadastroDto);
            if (erroValidacao is not null)
                return erroValidacao.Value;

            var codafListaPresenca = new CodafListaPresenca(
                codafListaPresencaCadastroDto.PropostaId,
                codafListaPresencaCadastroDto.PropostaTurmaId,
                codafListaPresencaCadastroDto.DataPublicacao,
                codafListaPresencaCadastroDto.DataPublicacaoDom,
                codafListaPresencaCadastroDto.NumeroComunicado,
                codafListaPresencaCadastroDto.PaginaComunicadoDom,
                codafListaPresencaCadastroDto.CodigoCursoEol,
                codafListaPresencaCadastroDto.CodigoNivel,
                codafListaPresencaCadastroDto.Observacao,
                contextoAplicacao.IdPerfilUsuario);
            codafListaPresenca.Iniciar();

            var idListaPresenca = await repositorioCodafListaPresenca.Inserir(codafListaPresenca);
            codafListaPresenca.Id = idListaPresenca;
            return mapper.Map<CodafListaPresencaDto>(codafListaPresenca);
        }
        private async Task<Erro?> ValidarRegrasDeNegocio(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto)
        {
            var proposta = await repositorioProposta.ObterPorId(codafListaPresencaCadastroDto.PropostaId);
            if (proposta is null)
                return Erro.Validacao("Proposta não encontrada.");

            var propostaTurma = await repositorioProposta.ObterTurmaPorId(codafListaPresencaCadastroDto.PropostaTurmaId);
            if (propostaTurma is null)
                return Erro.Validacao("Proposta Turma não encontrada.");

            if (propostaTurma.PropostaId != proposta.Id)
                return Erro.Validacao($"A turma não pertence à formação informada");

            var jaPossuiLista = await repositorioCodafListaPresenca
                .TurmaJaTemListaDePresencaAsync(codafListaPresencaCadastroDto.PropostaTurmaId);

            if (jaPossuiLista)
                return Erro.Negocio($"A turma {propostaTurma.Nome} já possui uma lista de presença cadastrada.");

            return null;
        }
    }
}
