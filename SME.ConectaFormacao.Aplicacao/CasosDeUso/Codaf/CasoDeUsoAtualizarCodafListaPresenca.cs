using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoAtualizarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioProposta repositorioProposta,
        IContextoAplicacao contextoAplicacao,
        IMapper mapper) :
        ICasoDeUsoAtualizarCodafListaPresenca
    {
        public async Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, int id)
        {
            var codafListaPresencaExistente = await repositorioCodafListaPresenca.ObterPorId(id);
            if (codafListaPresencaExistente is null)
                return Erro.Validacao("Lista de presença não encontrada.");

            var erroValidacao = await ValidarRegrasDeNegocio(codafListaPresencaEdicaoDto, id);
            if (erroValidacao is not null)
                return erroValidacao;

            codafListaPresencaExistente.AtualizarInformacoes(
                codafListaPresencaEdicaoDto.DataPublicacao,
                codafListaPresencaEdicaoDto.DataPublicacaoDom,
                codafListaPresencaEdicaoDto.NumeroComunicado,
                codafListaPresencaEdicaoDto.PaginaComunicadoDom,
                codafListaPresencaEdicaoDto.CodigoCursoEol,
                codafListaPresencaEdicaoDto.CodigoNivel,
                codafListaPresencaEdicaoDto.Observacao,
                contextoAplicacao.IdPerfilUsuario);

            await repositorioCodafListaPresenca.Atualizar(codafListaPresencaExistente);
            return mapper.Map<CodafListaPresencaDto>(codafListaPresencaExistente);
        }

        private async Task<Erro?> ValidarRegrasDeNegocio(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, int id)
        {
            var proposta = await repositorioProposta.ObterPorId(codafListaPresencaEdicaoDto.PropostaId);
            if (proposta is null)
                return Erro.Validacao("Proposta não encontrada.");

            var propostaTurma = await repositorioProposta.ObterTurmaPorId(codafListaPresencaEdicaoDto.PropostaTurmaId);
            if (propostaTurma is null)
                return Erro.Validacao("Proposta Turma não encontrada.");

            var jaPossuiLista = await repositorioCodafListaPresenca
                .TurmaJaTemListaDePresencaAsync(codafListaPresencaEdicaoDto.PropostaTurmaId, id);

            if (jaPossuiLista)
                return Erro.Negocio($"A turma {propostaTurma.Nome} já possui uma lista de presença cadastrada.");

            return null;
        }
    }
}
