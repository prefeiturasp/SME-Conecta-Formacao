using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
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
    public class Ao_obter_proposta : TesteBase
    {
        public Ao_obter_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido")]
        public async Task Deve_obter_por_id_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            ValidarProposta(propostaCompletoDTO, proposta.Id);
            ValidarPropostaPublicoAlvo(propostaCompletoDTO.PublicosAlvo, proposta.Id);
            ValidarPropostaFuncaoEspecifica(propostaCompletoDTO.FuncoesEspecificas, proposta.Id);
            ValidarPropostaVagaRemanecente(propostaCompletoDTO.VagasRemanecentes, proposta.Id);
            ValidarPropostaCriterioValidacaoInscricao(propostaCompletoDTO.CriteriosValidacaoInscricao, proposta.Id);

            ValidarAuditoria(proposta, propostaCompletoDTO.Auditoria);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção ao obter por id inválido")]
        public async Task Deve_retornar_excecao_ao_obter_por_id_invalido()
        {
            // arrange 
            var idAleatorio = PropostaSalvarMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

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

        private void ValidarProposta(PropostaCompletoDTO propostaDTO, long id)
        {
            var proposta = ObterPorId<Dominio.Entidades.Proposta, long>(id);

            proposta.TipoFormacao.ShouldBe(propostaDTO.TipoFormacao);
            proposta.Modalidade.ShouldBe(propostaDTO.Modalidade);
            proposta.TipoInscricao.ShouldBe(propostaDTO.TipoInscricao);
            proposta.NomeFormacao.ShouldBe(propostaDTO.NomeFormacao);
            proposta.QuantidadeTurmas.ShouldBe(propostaDTO.QuantidadeTurmas);
            proposta.QuantidadeVagasTurma.ShouldBe(propostaDTO.QuantidadeVagasTurma);

            if (!string.IsNullOrEmpty(propostaDTO.FuncaoEspecificaOutros))
                proposta.FuncaoEspecificaOutros.ShouldBe(propostaDTO.FuncaoEspecificaOutros);

            if (!string.IsNullOrEmpty(propostaDTO.CriterioValidacaoInscricaoOutros))
                proposta.CriterioValidacaoInscricaoOutros.ShouldBe(propostaDTO.CriterioValidacaoInscricaoOutros);

            proposta.Situacao.ShouldBe(propostaDTO.Situacao);
        }

        private void ValidarPropostaCriterioValidacaoInscricao(IEnumerable<PropostaCriterioValidacaoInscricaoDTO> criteriosDTO, long id)
        {
            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>().Where(t => !t.Excluido);
            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
            {
                criterioValidacaoInscricao.PropostaId.ShouldBe(id);
                criteriosDTO.FirstOrDefault(t => t.CriterioValidacaoInscricaoId == criterioValidacaoInscricao.CriterioValidacaoInscricaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaVagaRemanecente(IEnumerable<PropostaVagaRemanecenteDTO> vagasRemanecentesDTO, long id)
        {
            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>().Where(t => !t.Excluido);
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                vagaRemanecente.PropostaId.ShouldBe(id);
                vagasRemanecentesDTO.FirstOrDefault(t => t.CargoFuncaoId == vagaRemanecente.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaFuncaoEspecifica(IEnumerable<PropostaFuncaoEspecificaDTO> funcoesEspecificaDTO, long id)
        {
            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>().Where(t => !t.Excluido);
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                funcaoEspecifica.PropostaId.ShouldBe(id);
                funcoesEspecificaDTO.FirstOrDefault(t => t.CargoFuncaoId == funcaoEspecifica.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaPublicoAlvo(IEnumerable<PropostaPublicoAlvoDTO> publicosAlvoDTO, long id)
        {
            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>().Where(t => !t.Excluido);
            foreach (var publicoAlvo in publicosAlvo)
            {
                publicoAlvo.PropostaId.ShouldBe(id);
                publicosAlvoDTO.FirstOrDefault(t => t.CargoFuncaoId == publicoAlvo.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private static void ValidarAuditoria(Dominio.EntidadeBaseAuditavel entidadeAuditavel, AuditoriaDTO auditoriaDTO)
        {
            auditoriaDTO.CriadoEm.Date.ShouldBe(entidadeAuditavel.CriadoEm.Date);
            auditoriaDTO.CriadoEm.Hour.ShouldBe(entidadeAuditavel.CriadoEm.Hour);
            auditoriaDTO.CriadoPor.ShouldBe(entidadeAuditavel.CriadoPor);
            auditoriaDTO.CriadoLogin.ShouldBe(entidadeAuditavel.CriadoLogin);

            auditoriaDTO.AlteradoEm.GetValueOrDefault().Date.ShouldBe(entidadeAuditavel.AlteradoEm.GetValueOrDefault().Date);
            auditoriaDTO.AlteradoEm.GetValueOrDefault().Hour.ShouldBe(entidadeAuditavel.AlteradoEm.GetValueOrDefault().Hour);
            auditoriaDTO.AlteradoPor.ShouldBe(entidadeAuditavel.AlteradoPor);
            auditoriaDTO.AlteradoLogin.ShouldBe(entidadeAuditavel.AlteradoLogin);
        }
    }
}
