using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.CargoFuncao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_remover_proposta : TesteBase
    {
        public Ao_remover_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve remover por id válido")]
        public async Task Deve_remover_por_id_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverProposta>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert 
            retorno.ShouldBeTrue();

            var propostaBase = ObterTodos<Dominio.Entidades.Proposta>().First();
            propostaBase.Excluido.ShouldBeTrue();

            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>();
            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
                criterioValidacaoInscricao.Excluido.ShouldBeTrue();

            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>().Where(t => !t.Excluido);
            foreach (var vagaRemanecente in vagasRemanecentes)
                vagaRemanecente.Excluido.ShouldBeTrue();

            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>().Where(t => !t.Excluido);
            foreach (var funcaoEspecifica in funcoesEspecificas)
                funcaoEspecifica.Excluido.ShouldBeTrue();

            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>().Where(t => !t.Excluido);
            foreach (var publicoAlvo in publicosAlvo)
                publicoAlvo.Excluido.ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção ao remover por id inválido")]
        public async Task Deve_retornar_excecao_ao_remover_por_id_invalido()
        {
            // arrange 
            var idAleatorio = PropostaSalvarMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverProposta>();

            // act 
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(idAleatorio));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }

        private async Task<Dominio.Entidades.Proposta> InserirNaBaseProposta(Dominio.Entidades.AreaPromotora areaPromotora, IEnumerable<Dominio.Entidades.CargoFuncao> cargosFuncoes, IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            var proposta = PropostaMock.GerarPropostaValida(
                areaPromotora.Id,
                TipoFormacao.Curso,
                Modalidade.Presencial,
                SituacaoProposta.Ativo,
                false, false);

            await InserirNaBase(proposta);


            var publicosAlvo = PropostaMock.GerarPublicoAlvo(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo));
            if (publicosAlvo != null)
                await InserirNaBase(publicosAlvo);

            var funcoesEspecifica = PropostaMock.GerarFuncoesEspecificas(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao));
            if (funcoesEspecifica != null)
                await InserirNaBase(funcoesEspecifica);

            var vagasRemanecentes = PropostaMock.GerarVagasRemanecentes(proposta.Id, cargosFuncoes);
            if (vagasRemanecentes != null)
                await InserirNaBase(vagasRemanecentes);

            var criterios = PropostaMock.GerarCritariosValidacaoInscricao(proposta.Id, criteriosValidacaoInscricao);
            if (criterios != null)
                await InserirNaBase(criterios);

            return proposta;
        }
    }
}
