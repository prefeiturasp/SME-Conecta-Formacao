using AutoMapper;
using FluentValidation;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoCriarCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IRepositorioCodafListaPresenca repositorioCodafLista,
        IValidator<CodafSuplementarCadastroDto> validator,
        IMapper mapper) : ICasoDeUsoCriarCodafSuplementar
    {
        public async Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto)
        {
            var validationResult = await validator.ValidateAsync(codafSuplementarCadastroDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            var codafOriginal = await repositorioCodafLista.ObterNaoExcluidosPorIdAsync(codafSuplementarCadastroDto.CodafId);

            if (codafOriginal is null)
                return Erro.Validacao("Codaf não encontrado para a turma informada");

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

            return mapper.Map<CodafSuplementarDetalhadoDto>(codafSuplementar);
        }
    }
}
