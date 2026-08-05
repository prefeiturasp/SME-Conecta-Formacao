using AutoMapper;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias
{
    public record CodafCursoNaoHomologadoDependencias(
        IRepositorioCodafCursoNaoHomologado RepositorioCodaf,
        ICodafCursoNaoHomologadoInscritosService InscritosService,
        IGerenciadorAnexosCodafCursoNaoHomologadoService AnexoService,
        IMapper Mapper,
        ITransacao Transacao);
}
