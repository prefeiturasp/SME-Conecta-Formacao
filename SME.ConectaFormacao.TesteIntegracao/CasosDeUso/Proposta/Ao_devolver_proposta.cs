using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;

public class Ao_devolver_proposta : TestePropostaBase
{
    public Ao_devolver_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
    {
    }

    protected override void RegistrarFakes(IServiceCollection services)
    {
        base.RegistrarFakes(services);
        services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));        
    }

    [Fact(DisplayName = "Proposta - Deve devolver proposta válida")]
    public async Task Deve_devolver_proposta_valida()
    {
        //arrange
        var parametroQtdeCursistasSuportadosPorTurma = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, "950");
        await InserirNaBase(parametroQtdeCursistasSuportadosPorTurma);

        var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
        await InserirNaBase(areaPromotora);

        var dres = DreMock.GerarDreValida(5);
        await InserirNaBase(dres);

        var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
        await InserirNaBase(cargosFuncoes);

        var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
        await InserirNaBase(criteriosValidacaoInscricao);

        var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
        await InserirNaBase(palavrasChaves);

        var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

        var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
        await InserirNaBase(anosTurmas);

        var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
        await InserirNaBase(componentesCurriculares);

        var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,
            palavrasChaves, modalidades, anosTurmas, componentesCurriculares);

        var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
            TipoFormacao.Curso,
            Formato.Presencial,
            dres.Select(t => new PropostaDreDTO { DreId = t.Id }),
            cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id }),
            cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id }),
            criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id }),
            cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id }),
            palavrasChaves.Select(t => new PropostaPalavraChaveDTO { PalavraChaveId = t.Id }),
            modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t }),
            anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id }),
            componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO { ComponenteCurricularId = t.Id }),
            SituacaoProposta.Cadastrada, quantidadeTurmas: proposta.QuantidadeTurmas);

        propostaDTO.Turmas.FirstOrDefault().Id = proposta.Turmas.FirstOrDefault().Id;

        var casoDeUso = ObterCasoDeUso<ICasoDeUsoDevolverProposta>();
        
        // act 
        var retornoDto = await casoDeUso.Executar(proposta.Id, "Justificativa do teste integrado ao devolver a proposta.");
        
        // assert
        retornoDto.ShouldBeTrue();        
    }
    
    [Fact(DisplayName = "Proposta - Deve validar preenchimento da justificativa da proposta válida")]
    public async Task Deve_validar_preenchimento_justificativa_proposta_valida()
    {
        //arrange
        var parametroQtdeCursistasSuportadosPorTurma = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, "950");
        await InserirNaBase(parametroQtdeCursistasSuportadosPorTurma);

        var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
        await InserirNaBase(areaPromotora);

        var dres = DreMock.GerarDreValida(5);
        await InserirNaBase(dres);

        var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
        await InserirNaBase(cargosFuncoes);

        var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
        await InserirNaBase(criteriosValidacaoInscricao);

        var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
        await InserirNaBase(palavrasChaves);

        var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

        var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
        await InserirNaBase(anosTurmas);

        var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
        await InserirNaBase(componentesCurriculares);

        var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,
            palavrasChaves, modalidades, anosTurmas, componentesCurriculares);

        var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
            TipoFormacao.Curso,
            Formato.Presencial,
            dres.Select(t => new PropostaDreDTO { DreId = t.Id }),
            cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id }),
            cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id }),
            criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id }),
            cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id }),
            palavrasChaves.Select(t => new PropostaPalavraChaveDTO { PalavraChaveId = t.Id }),
            modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t }),
            anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id }),
            componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO { ComponenteCurricularId = t.Id }),
            SituacaoProposta.Cadastrada, quantidadeTurmas: proposta.QuantidadeTurmas);

        propostaDTO.Turmas.FirstOrDefault().Id = proposta.Turmas.FirstOrDefault().Id;

        var casoDeUso = ObterCasoDeUso<ICasoDeUsoDevolverProposta>();
        
        // act 
        var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, ""));
        
        // assert
        excecao.ShouldNotBeNull();
        excecao.StatusCode.ShouldBe(400);
        excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);        
    }    
}