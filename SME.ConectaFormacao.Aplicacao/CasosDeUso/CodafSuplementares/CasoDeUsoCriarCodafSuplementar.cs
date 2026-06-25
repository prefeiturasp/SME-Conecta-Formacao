using FluentValidation;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    [ExcludeFromCodeCoverage]
    public class CasoDeUsoCriarCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IRepositorioCodafListaPresenca repositorioCodafLista,
        IValidator<CodafSuplementarCadastroDto> validator) : ICasoDeUsoCriarCodafSuplementar
    {
        public async Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto)
        {
            var validationResult = await validator.ValidateAsync(codafSuplementarCadastroDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            var codafOriginal = await repositorioCodafLista.ObterNaoExcluidosPorIdAsync(codafSuplementarCadastroDto.CodafId);

            if (codafOriginal is null)
                return Erro.Validacao("Codaf não encontrado para a turma informada");

            var codafSuplementarExistente = await repositorioCodafSuplementar.ObterPorExpressaoAsync(c => c.CodafId == codafOriginal.Id);

            if (codafSuplementarExistente is not null)
                return Erro.Validacao("Já existe um codaf suplementar para a turma informada");

            var codafSuplementar = new CodafSuplementar(codafSuplementarCadastroDto.CodafId,
                new(codafSuplementarCadastroDto.DataPublicacao,
                    codafSuplementarCadastroDto.DataPublicacaoDom,
                    codafSuplementarCadastroDto.NumeroComunicado,
                    codafSuplementarCadastroDto.PaginaComunicadoDom,
                    codafSuplementarCadastroDto.CodigoCursoEol,
                    codafSuplementarCadastroDto.CodigoNivel,
                    codafSuplementarCadastroDto.Observacao));

            var id = await repositorioCodafSuplementar.Inserir(codafSuplementar);
            codafSuplementar.Id = id;

            var codafSuplementarDetalhadoDto = new CodafSuplementarDetalhadoDto
            {
                AlteradoEm = codafSuplementar.AlteradoEm,
                AlteradoLogin = codafSuplementar.AlteradoLogin,
                AlteradoPor = codafSuplementar.AlteradoPor,
                CodigoCursoEol = codafSuplementar.CodigoCursoEol,
                CodigoNivel = codafSuplementar.CodigoNivel,
                CriadoEm = codafSuplementar.CriadoEm,
                CriadoLogin = codafSuplementar.CriadoLogin,
                CriadoPor = codafSuplementar.CriadoPor,
                DataPublicacao = codafSuplementar.DataPublicacao,
                DataPublicacaoDom = codafSuplementar.DataPublicacaoDom,
                Id = codafSuplementar.Id,
                CodigoFormacao = codafOriginal.PropostaId,
                PropostaId = codafOriginal.PropostaId
            };

            return codafSuplementarDetalhadoDto;
        }
    }
}
