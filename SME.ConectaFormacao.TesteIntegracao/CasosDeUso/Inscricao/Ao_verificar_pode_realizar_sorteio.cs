using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_verificar_pode_realizar_sorteio : TestePropostaBase
    {
        public Ao_verificar_pode_realizar_sorteio(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Inscrição - Deve permitir realizar sorteio quando tem mais inscritos que vagas em propostas homologadas e com critério de sorteio")]
        public async Task Deve_permitir_realizar_sorteio_quando_tem_mais_inscritos_que_vagas_em_propostas_homologadas_e_com_criterios_de_sorteio()
        {
            // arrange
            var usuarioInscrito = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioInscrito);

            var usuarioAdicional = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioAdicional);
            
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuarioInscrito.Id, propostaTurma.Id);
            await InserirNaBase(inscricao);
            
            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id, inscricao.Id);
            await InserirNaBase(vaga);
            
            inscricao = InscricaoMock.GerarInscricao(usuarioAdicional.Id, propostaTurma.Id);
            await InserirNaBase(inscricao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoPodeRealizarSorteioPorId>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert 
            retorno.ShouldBeTrue();
            
            var inscricoes = ObterTodos<Dominio.Entidades.Inscricao>();
            inscricoes.Count.ShouldBe(2);
            
            var propostaTurmaVagas = ObterTodos<PropostaTurmaVaga>();
            propostaTurmaVagas.Count.ShouldBe(1);
        }
        
        [Fact(DisplayName = "Inscrição - Não deve permitir realizar sorteio quando tem mais inscritos que vagas em propostas não homologadas e com critério de sorteio")]
        public async Task Nao_deve_permitir_realizar_sorteio_quando_tem_mais_inscritos_que_vagas_em_propostas_nao_homologadas_e_com_criterios_de_sorteio()
        {
            // arrange
            var usuarioInscrito = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioInscrito);

            var usuarioAdicional = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioAdicional);
            
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosExtras);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuarioInscrito.Id, propostaTurma.Id);
            await InserirNaBase(inscricao);
            
            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id, inscricao.Id);
            await InserirNaBase(vaga);
            
            inscricao = InscricaoMock.GerarInscricao(usuarioAdicional.Id, propostaTurma.Id);
            await InserirNaBase(inscricao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoPodeRealizarSorteioPorId>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert 
            retorno.ShouldBeFalse();
            
            var inscricoes = ObterTodos<Dominio.Entidades.Inscricao>();
            inscricoes.Count.ShouldBe(2);
            
            var propostaTurmaVagas = ObterTodos<PropostaTurmaVaga>();
            propostaTurmaVagas.Count.ShouldBe(1);
        }
        
        [Fact(DisplayName = "Inscrição - Não deve permitir realizar sorteio quando tem mesmo número de inscritos que vagas em propostas homologadas e com critério de sorteio")]
        public async Task Nao_deve_permitir_realizar_sorteio_quando_tem_mesmo_numero_de_vagas_que_inscritos_que_vagas_em_propostas_nao_homologadas_e_com_criterios_de_sorteio()
        {
            // arrange
            var usuarioInscrito = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioInscrito);

            var usuarioAdicional = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioAdicional);
            
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosExtras);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuarioInscrito.Id, propostaTurma.Id);
            await InserirNaBase(inscricao);
            
            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id, inscricao.Id);
            await InserirNaBase(vaga);
            
            inscricao = InscricaoMock.GerarInscricao(usuarioAdicional.Id, propostaTurma.Id);
            await InserirNaBase(inscricao);
            
            vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id, inscricao.Id);
            await InserirNaBase(vaga);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoPodeRealizarSorteioPorId>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert 
            retorno.ShouldBeFalse();
            
            var inscricoes = ObterTodos<Dominio.Entidades.Inscricao>();
            inscricoes.Count.ShouldBe(2);
            
            var propostaTurmaVagas = ObterTodos<PropostaTurmaVaga>();
            propostaTurmaVagas.Count.ShouldBe(2);
        }
    }
}
