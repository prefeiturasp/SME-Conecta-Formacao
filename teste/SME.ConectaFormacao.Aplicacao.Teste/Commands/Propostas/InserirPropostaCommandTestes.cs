using FluentValidation.TestHelper;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class InserirPropostaCommandValidatorTests
    {
        private readonly InserirPropostaCommandValidator validator;

        public InserirPropostaCommandValidatorTests()
        {
            validator = new InserirPropostaCommandValidator();
        }

        [Fact]
        public void Deve_preencher_command_no_construtor()
        {
            var dto = CriarDtoValido();

            var command = new InserirPropostaCommand(10, dto);

            Assert.Equal(10, command.AreaPromotoraId);
            Assert.Equal(dto, command.PropostaDTO);
        }

        [Fact]
        public void Deve_retornar_erro_quando_area_promotora_for_invalida()
        {
            var dto = CriarDtoValido();

            var command = new InserirPropostaCommand(0, dto);

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.AreaPromotoraId);
        }

        [Fact]
        public void Deve_retornar_erro_quando_tipo_formacao_nao_for_informado()
        {
            var dto = CriarDtoValido();
            dto.TipoFormacao = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("TipoFormacao");
        }

        [Fact]
        public void Deve_retornar_erro_quando_formato_nao_for_informado()
        {
            var dto = CriarDtoValido();
            dto.Formato = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("Formato");
        }

        [Fact]
        public void Deve_retornar_erro_quando_curso_for_hibrido()
        {
            var dto = CriarDtoValido();
            dto.TipoFormacao = TipoFormacao.Curso;
            dto.Formato = Formato.Hibrido;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("Formato");
        }

        [Fact]
        public void Deve_nao_retornar_erro_para_evento_hibrido()
        {
            var dto = CriarDtoValido();
            dto.TipoFormacao = TipoFormacao.Evento;
            dto.Formato = Formato.Hibrido;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldNotHaveValidationErrorFor("Formato");
        }

        [Fact]
        public void Deve_exigir_id_emissor_quando_curso_com_certificado()
        {
            var dto = CriarDtoValido();

            dto.CursoComCertificado = true;
            dto.IdEmissor = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("IdEmissor");
        }

        [Fact]
        public void Deve_exigir_id_emissor_maior_que_zero()
        {
            var dto = CriarDtoValido();

            dto.CursoComCertificado = true;
            dto.IdEmissor = 0;
            dto.TipoEmissor = TipoEmissor.Coordenadoria;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("IdEmissor");
        }

        [Fact]
        public void Deve_exigir_tipo_emissor()
        {
            var dto = CriarDtoValido();

            dto.CursoComCertificado = true;
            dto.IdEmissor = 10;
            dto.TipoEmissor = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("TipoEmissor");
        }

        [Fact]
        public void Nao_deve_validar_tipo_emissor_quando_sem_certificado()
        {
            var dto = CriarDtoValido();

            dto.CursoComCertificado = false;
            dto.IdEmissor = null;
            dto.TipoEmissor = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldNotHaveValidationErrorFor("IdEmissor");
            result.ShouldNotHaveValidationErrorFor("TipoEmissor");
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
            var dto = CriarDtoValido();

            switch (campo)
            {
                case "Dres":
                    dto.Dres = [];
                    break;

                case "Criterios":
                    dto.CriteriosValidacaoInscricao = [];
                    break;

                case "Turmas":
                    dto.Turmas = [];
                    break;

                case "Justificativa":
                    dto.Justificativa = "";
                    break;

                case "Objetivos":
                    dto.Objetivos = "";
                    break;

                case "Conteudo":
                    dto.ConteudoProgramatico = "";
                    break;

                case "Procedimento":
                    dto.ProcedimentoMetadologico = "";
                    break;

                case "Referencia":
                    dto.Referencia = "";
                    break;

                case "Palavras":
                    dto.PalavrasChaves = null!;
                    break;
            }

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Deve_validar_quantidade_turmas()
        {
            var dto = CriarDtoValido();
            dto.QuantidadeTurmas = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("QuantidadeTurmas");
        }

        [Fact]
        public void Deve_validar_quantidade_vagas()
        {
            var dto = CriarDtoValido();
            dto.QuantidadeVagasTurma = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("QuantidadeVagasTurma");
        }

        [Fact]
        public void Deve_exigir_link_quando_inscricao_externa()
        {
            var dto = CriarDtoValido();

            dto.TiposInscricao =
            [
                new PropostaTipoInscricaoDTO
                {
                    TipoInscricao = TipoInscricao.Externa
                }
            ];

            dto.LinkParaInscricoesExterna = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldHaveValidationErrorFor("LinkParaInscricoesExterna");
        }

        [Fact]
        public void Nao_deve_exigir_link_quando_inscricao_nao_for_externa()
        {
            var dto = CriarDtoValido();

            dto.TiposInscricao =
            [
                new PropostaTipoInscricaoDTO
                {
                    TipoInscricao = TipoInscricao.Automatica
                }
            ];

            dto.LinkParaInscricoesExterna = null;

            var result = validator.TestValidate(new InserirPropostaCommand(1, dto));

            result.ShouldNotHaveValidationErrorFor("LinkParaInscricoesExterna");
        }

        [Fact]
        public void Deve_validar_sem_erros()
        {
            var dto = CriarDtoValido();

            var result = validator.TestValidate(new InserirPropostaCommand(10, dto));

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static PropostaDTO CriarDtoValido()
        {
            return new PropostaDTO
            {
                TipoFormacao = TipoFormacao.Evento,
                Formato = Formato.Presencial,
                QuantidadeTurmas = 1,
                QuantidadeVagasTurma = 10,

                Justificativa = "Justificativa",
                Objetivos = "Objetivos",
                ConteudoProgramatico = "Conteúdo",
                ProcedimentoMetadologico = "Procedimento",
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
                        TipoInscricao = TipoInscricao.Automatica
                    }
                ]
            };
        }
    }
}
