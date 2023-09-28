using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public abstract class TestePropostaBase : TesteBase
    {
        protected TestePropostaBase(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected async Task<Dominio.Entidades.Proposta> InserirNaBaseProposta(Dominio.Entidades.AreaPromotora areaPromotora, 
            IEnumerable<Dominio.Entidades.CargoFuncao> cargosFuncoes, IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao,
            IEnumerable<PalavraChave> palavrasChaves)
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
            {
                await InserirNaBase(publicosAlvo);
                proposta.PublicosAlvo = publicosAlvo;
            }

            var funcoesEspecifica = PropostaMock.GerarFuncoesEspecificas(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao));
            if (funcoesEspecifica != null)
            {
                await InserirNaBase(funcoesEspecifica);
                proposta.FuncoesEspecificas = funcoesEspecifica;
            }

            var vagasRemanecentes = PropostaMock.GerarVagasRemanecentes(proposta.Id, cargosFuncoes);
            if (vagasRemanecentes != null)
            {
                await InserirNaBase(vagasRemanecentes);
                proposta.VagasRemanecentes = vagasRemanecentes;
            }

            var criterios = PropostaMock.GerarCritariosValidacaoInscricao(proposta.Id, criteriosValidacaoInscricao);
            if (criterios != null)
            {
                await InserirNaBase(criterios);
                proposta.CriteriosValidacaoInscricao = criterios;
            }

            var encontros = PropostaMock.GerarEncontros(proposta.Id);
            if (encontros != null)
            {
                await InserirNaBase(encontros);

                foreach (var encontro in encontros)
                {
                    var turmas = PropostaMock.GerarPropostaEncontroTurmas(encontro.Id, proposta.QuantidadeTurmas.GetValueOrDefault());
                    await InserirNaBase(turmas);
                    encontro.Turmas = turmas;

                    var datas = PropostaMock.GerarPropostaEncontroDatas(encontro.Id);
                    await InserirNaBase(datas);
                    encontro.Datas = datas;
                }
                
                var palavrasChavesDaProposta = PropostaMock.GerarPalavrasChaves(proposta.Id, palavrasChaves);
                if (palavrasChavesDaProposta != null)
                {
                    await InserirNaBase(palavrasChavesDaProposta);
                    proposta.PalavrasChaves = palavrasChavesDaProposta;
                }

                proposta.Encontros = encontros;
            }

            return proposta;
        }

        protected async Task<IEnumerable<Dominio.Entidades.Proposta>> InserirNaBaseProposta(int quantidade, 
            Dominio.Entidades.AreaPromotora areaPromotora, IEnumerable<Dominio.Entidades.CargoFuncao> cargosFuncoes, 
            IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao,IEnumerable<PalavraChave> palavrasChaves)
        {
            var lista = new List<Dominio.Entidades.Proposta>();

            for (int i = 0; i < quantidade; i++)
            {
                lista.Add(await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves));
            }

            return lista;
        }

        protected void ValidarPropostaDTO(PropostaDTO propostaDTO, long id)
        {
            var proposta = ObterPorId<Dominio.Entidades.Proposta, long>(id);

            proposta.TipoFormacao.ShouldBe(propostaDTO.TipoFormacao);
            proposta.Modalidade.ShouldBe(propostaDTO.Modalidade);
            proposta.TipoInscricao.ShouldBe(propostaDTO.TipoInscricao);
            proposta.NomeFormacao.ShouldBe(propostaDTO.NomeFormacao);
            proposta.QuantidadeTurmas.ShouldBe(propostaDTO.QuantidadeTurmas);
            proposta.QuantidadeVagasTurma.ShouldBe(propostaDTO.QuantidadeVagasTurma);
            proposta.Justificativa.ShouldBe(propostaDTO.Justificativa);
            proposta.Objetivos.ShouldBe(propostaDTO.Objetivos);
            proposta.ConteudoProgramatico.ShouldBe(propostaDTO.ConteudoProgramatico);
            proposta.ProcedimentoMetadologico.ShouldBe(propostaDTO.ProcedimentoMetadologico);
            proposta.Referencia.ShouldBe(propostaDTO.Referencia);
            
            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaPresencial))
                proposta.CargaHorariaPresencial.ShouldBe(propostaDTO.CargaHorariaPresencial);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaSincrona))
                proposta.CargaHorariaSincrona.ShouldBe(propostaDTO.CargaHorariaSincrona);
            
            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaDistancia))
                proposta.CargaHorariaDistancia.ShouldBe(propostaDTO.CargaHorariaDistancia);
            
            if (!string.IsNullOrEmpty(propostaDTO.FuncaoEspecificaOutros))
                proposta.FuncaoEspecificaOutros.ShouldBe(propostaDTO.FuncaoEspecificaOutros);

            if (!string.IsNullOrEmpty(propostaDTO.CriterioValidacaoInscricaoOutros))
                proposta.CriterioValidacaoInscricaoOutros.ShouldBe(propostaDTO.CriterioValidacaoInscricaoOutros);
            
            proposta.Situacao.ShouldBe(propostaDTO.Situacao);
        }

        protected void ValidarPropostaCompletoDTO(PropostaCompletoDTO propostaDTO, long id)
        {
            var proposta = ObterPorId<Dominio.Entidades.Proposta, long>(id);

            proposta.TipoFormacao.ShouldBe(propostaDTO.TipoFormacao);
            proposta.Modalidade.ShouldBe(propostaDTO.Modalidade);
            proposta.TipoInscricao.ShouldBe(propostaDTO.TipoInscricao);
            proposta.NomeFormacao.ShouldBe(propostaDTO.NomeFormacao);
            proposta.QuantidadeTurmas.ShouldBe(propostaDTO.QuantidadeTurmas);
            proposta.QuantidadeVagasTurma.ShouldBe(propostaDTO.QuantidadeVagasTurma);
            proposta.Justificativa.ShouldBe(propostaDTO.Justificativa);
            proposta.Objetivos.ShouldBe(propostaDTO.Objetivos);
            proposta.ConteudoProgramatico.ShouldBe(propostaDTO.ConteudoProgramatico);
            proposta.ProcedimentoMetadologico.ShouldBe(propostaDTO.ProcedimentoMetadologico);
            proposta.Referencia.ShouldBe(propostaDTO.Referencia);
            
            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaPresencial))
                proposta.CargaHorariaPresencial.ShouldBe(propostaDTO.CargaHorariaPresencial);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaSincrona))
                proposta.CargaHorariaSincrona.ShouldBe(propostaDTO.CargaHorariaSincrona);
            
            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaDistancia))
                proposta.CargaHorariaDistancia.ShouldBe(propostaDTO.CargaHorariaDistancia);
            
            if (!string.IsNullOrEmpty(propostaDTO.FuncaoEspecificaOutros))
                proposta.FuncaoEspecificaOutros.ShouldBe(propostaDTO.FuncaoEspecificaOutros);

            if (!string.IsNullOrEmpty(propostaDTO.CriterioValidacaoInscricaoOutros))
                proposta.CriterioValidacaoInscricaoOutros.ShouldBe(propostaDTO.CriterioValidacaoInscricaoOutros);
            
            proposta.Situacao.ShouldBe(propostaDTO.Situacao);
        }

        protected void ValidarPropostaCriterioValidacaoInscricaoDTO(IEnumerable<PropostaCriterioValidacaoInscricaoDTO> criteriosDTO, long id)
        {
            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>().Where(t => !t.Excluido);
            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
            {
                criterioValidacaoInscricao.PropostaId.ShouldBe(id);
                criteriosDTO.FirstOrDefault(t => t.CriterioValidacaoInscricaoId == criterioValidacaoInscricao.CriterioValidacaoInscricaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaVagaRemanecenteDTO(IEnumerable<PropostaVagaRemanecenteDTO> vagasRemanecentesDTO, long id)
        {
            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>().Where(t => !t.Excluido);
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                vagaRemanecente.PropostaId.ShouldBe(id);
                vagasRemanecentesDTO.FirstOrDefault(t => t.CargoFuncaoId == vagaRemanecente.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaFuncaoEspecificaDTO(IEnumerable<PropostaFuncaoEspecificaDTO> funcoesEspecificaDTO, long id)
        {
            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>().Where(t => !t.Excluido);
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                funcaoEspecifica.PropostaId.ShouldBe(id);
                funcoesEspecificaDTO.FirstOrDefault(t => t.CargoFuncaoId == funcaoEspecifica.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaPublicoAlvoDTO(IEnumerable<PropostaPublicoAlvoDTO> publicosAlvoDTO, long id)
        {
            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>().Where(t => !t.Excluido);
            foreach (var publicoAlvo in publicosAlvo)
            {
                publicoAlvo.PropostaId.ShouldBe(id);
                publicosAlvoDTO.FirstOrDefault(t => t.CargoFuncaoId == publicoAlvo.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaEncontro(IEnumerable<PropostaEncontroDTO> encontrosDTO, long id)
        {
            var encontros = ObterTodos<PropostaEncontro>();
            var turmas = ObterTodos<PropostaEncontroTurma>();
            var datas = ObterTodos<PropostaEncontroData>();

            foreach (var encontroDTO in encontrosDTO)
            {
                var encontro = encontros.FirstOrDefault(t =>
                    t.PropostaId == id &&
                    t.HoraInicio == encontroDTO.HoraInicio &&
                    t.HoraFim == encontroDTO.HoraFim &&
                    t.Local == encontroDTO.Local
                    );
                encontro.ShouldNotBeNull();

                foreach (var dataDTO in encontroDTO.Datas)
                {
                    var data = datas.FirstOrDefault(t =>
                        t.PropostaEncontroId == encontro.Id &&
                        t.DataInicio.GetValueOrDefault().Date == dataDTO.DataInicio.Date &&
                        t.DataFim.GetValueOrDefault().Date == dataDTO.DataFim.GetValueOrDefault().Date);
                    data.ShouldNotBeNull();
                }
            }
        }

        protected static void ValidarAuditoriaDTO(Dominio.EntidadeBaseAuditavel entidadeAuditavel, AuditoriaDTO auditoriaDTO)
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
        
        protected void ValidarPropostaPalavrasChavesDTO(IEnumerable<PropostaPalavraChaveDTO> propostaPalavrasChavesDTO, long id)
        {
            var palavrasChaves = ObterTodos<PropostaPalavraChave>().Where(t => !t.Excluido);
            foreach (var palavraChave in palavrasChaves)
            {
                palavraChave.PropostaId.ShouldBe(id);
                propostaPalavrasChavesDTO.Any(t => t.PalavraChaveId == palavraChave.Id).ShouldBeTrue();
            }
        }
    }
}
