using FluentValidation.TestHelper;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class AlterarPropostaCommandValidatorTestes
    {
        private readonly AlterarPropostaCommandValidator _validator;

        public AlterarPropostaCommandValidatorTestes()
        {
            _validator = new AlterarPropostaCommandValidator();
        }

        private AlterarPropostaCommand CriarComandoValido()
        {
            return new AlterarPropostaCommand(1, CriarPropostaDtoValida());
        }

        private static PropostaDTO CriarPropostaDtoValida()
        {
            return new PropostaDTO
            {
                TipoFormacao = TipoFormacao.Curso,
                Formato = Formato.Presencial,
                QuantidadeTurmas = 1,
                QuantidadeVagasTurma = 10,

                Justificativa = "Justificativa",
                Objetivos = "Objetivos",
                ConteudoProgramatico = "Conteúdo",
                ProcedimentoMetadologico = "Procedimentos",
                Referencia = "Referência",
                SobreEsteCurso = "Sobre este curso",

                Dres =
                [
                    new PropostaDreDTO()
                ],

                Turmas =
                [
                    new PropostaTurmaDTO()
                ],

                PalavrasChaves =
                [
                    new PropostaPalavraChaveDTO()
                ],

                CriteriosValidacaoInscricao =
                [
                    new PropostaCriterioValidacaoInscricaoDTO()
                ],

                TiposInscricao =
                [
                    new PropostaTipoInscricaoDTO
                    {
                        TipoInscricao = TipoInscricao.Optativa
                    }
                ]
            };
        }

        [Fact]
        public void Deve_preencher_command_no_construtor()
        {
            var dto = CriarPropostaDtoValida();

            var command = new AlterarPropostaCommand(10, dto);

            Assert.Equal(10, command.Id);
            Assert.Equal(dto, command.PropostaDTO);
        }

        [Fact]
        public void DadoComandoValido_QuandoValidar_EntaoNaoDeveTerErros()
        {
            var comando = CriarComandoValido();
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_retornar_erro_quando_id_for_invalido()
        {
            var comando = CriarComandoValido();
            comando.Id = 0;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("É necessário informar o Id para alterar a proposta");
        }

        [Fact]
        public void Deve_retornar_erro_quando_tipo_formacao_nao_for_informado()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TipoFormacao = null;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor("TipoFormacao");
        }

        [Fact]
        public void Deve_retornar_erro_quando_formato_nao_for_informado()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.Formato = null;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor("Formato");
        }

        [Fact]
        public void Deve_retornar_erro_quando_curso_for_hibrido()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TipoFormacao = TipoFormacao.Curso;
            comando.PropostaDTO.Formato = Formato.Hibrido;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor("Formato");
        }

        [Fact]
        public void Deve_nao_retornar_erro_para_evento_hibrido()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TipoFormacao = TipoFormacao.Evento;
            comando.PropostaDTO.Formato = Formato.Hibrido;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldNotHaveValidationErrorFor("Formato");
        }

        [Theory]
        [InlineData("Dres")]
        [InlineData("Criterios")]
        [InlineData("Turmas")]
        [InlineData("Justificativa")]
        [InlineData("Objetivos")]
        [InlineData("Conteudo")]
        [InlineData("Procedimento")]
        [InlineData("Referencia")]
        [InlineData("Palavras")]
        public void Deve_validar_campos_obrigatorios(string campo)
        {
            var comando = CriarComandoValido();

            switch (campo)
            {
                case "Dres":
                    comando.PropostaDTO.Dres = [];
                    break;

                case "Criterios":
                    comando.PropostaDTO.CriteriosValidacaoInscricao = [];
                    break;

                case "Turmas":
                    comando.PropostaDTO.Turmas = [];
                    break;

                case "Justificativa":
                    comando.PropostaDTO.Justificativa = "";
                    break;

                case "Objetivos":
                    comando.PropostaDTO.Objetivos = "";
                    break;

                case "Conteudo":
                    comando.PropostaDTO.ConteudoProgramatico = "";
                    break;

                case "Procedimento":
                    comando.PropostaDTO.ProcedimentoMetadologico = "";
                    break;

                case "Referencia":
                    comando.PropostaDTO.Referencia = "";
                    break;

                case "Palavras":
                    comando.PropostaDTO.PalavrasChaves = null!;
                    break;
            }

            var resultado = _validator.TestValidate(comando);
            Assert.False(resultado.IsValid);
        }

        [Fact]
        public void Deve_validar_quantidade_turmas()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.QuantidadeTurmas = null;

            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor("QuantidadeTurmas");
        }

        [Fact]
        public void Deve_validar_quantidade_vagas()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.QuantidadeVagasTurma = null;

            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor("QuantidadeVagasTurma");
        }

        [Fact]
        public void Deve_exigir_link_quando_inscricao_externa()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TiposInscricao =
            [
                new PropostaTipoInscricaoDTO
                {
                    TipoInscricao = TipoInscricao.Externa
                }
            ];

            comando.PropostaDTO.LinkParaInscricoesExterna = null;

            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor("LinkParaInscricoesExterna");
        }

        [Fact]
        public void Nao_deve_exigir_link_quando_inscricao_nao_for_externa()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TiposInscricao =
            [
                new PropostaTipoInscricaoDTO
                {
                    TipoInscricao = TipoInscricao.Automatica
                }
            ];

            comando.PropostaDTO.LinkParaInscricoesExterna = null;

            var resultado = _validator.TestValidate(comando);
            resultado.ShouldNotHaveValidationErrorFor("LinkParaInscricoesExterna");
        }
    }
}
