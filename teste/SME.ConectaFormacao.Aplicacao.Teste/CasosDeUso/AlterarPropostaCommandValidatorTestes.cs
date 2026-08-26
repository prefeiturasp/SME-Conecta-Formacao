using FluentValidation.TestHelper;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using Xunit;
using System.Collections.Generic;

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
            var dto = new PropostaDTO
            {
                TipoFormacao = TipoFormacao.Curso,
                Formato = Formato.Presencial,
                TiposInscricao = new List<PropostaTipoInscricaoDTO> { new PropostaTipoInscricaoDTO { TipoInscricao = TipoInscricao.Optativa } },
                Dres = new List<PropostaDreDTO> { new PropostaDreDTO() },
                CriteriosValidacaoInscricao = new List<PropostaCriterioValidacaoInscricaoDTO> { new PropostaCriterioValidacaoInscricaoDTO() },
                QuantidadeTurmas = 1,
                QuantidadeVagasTurma = 10,
                Turmas = new List<PropostaTurmaDTO> { new PropostaTurmaDTO() },
                Justificativa = "Justificativa",
                Objetivos = "Objetivos",
                ConteudoProgramatico = "Conteúdo",
                ProcedimentoMetadologico = "Procedimentos",
                Referencia = "Referência",
                PalavrasChaves = new List<PropostaPalavraChaveDTO> { new PropostaPalavraChaveDTO() }
            };

            return new AlterarPropostaCommand(1, dto);
        }

        [Fact]
        public void DadoComandoValido_QuandoValidar_EntaoNaoDeveTerErros()
        {
            var comando = CriarComandoValido();
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void DadoComandoComIdInvalido_QuandoValidar_EntaoDeveTerErro()
        {
            var comando = CriarComandoValido();
            comando.Id = 0;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("É necessário informar o Id para alterar a proposta");
        }

        [Fact]
        public void DadoComandoSemTipoFormacao_QuandoValidar_EntaoDeveTerErro()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TipoFormacao = null;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.TipoFormacao);
        }

        [Fact]
        public void DadoComandoSemFormato_QuandoValidar_EntaoDeveTerErro()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.Formato = null;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.Formato);
        }

        [Fact]
        public void DadoComandoCursoComFormatoHibrido_QuandoValidar_EntaoDeveTerErro()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TipoFormacao = TipoFormacao.Curso;
            comando.PropostaDTO.Formato = Formato.Hibrido;
            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.Formato).WithErrorMessage("É permitido o formato Híbrido somente para o tipo de formação evento");
        }

        [Fact]
        public void DadoCursoComCertificadoMasSemEmissor_QuandoValidar_EntaoDeveTerErro()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.CursoComCertificado = true;
            comando.PropostaDTO.IdEmissor = null;
            comando.PropostaDTO.TipoEmissor = null;

            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.IdEmissor);
            resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.TipoEmissor);
        }

        [Fact]
        public void DadoComandoComInscricaoExternaSemLink_QuandoValidar_EntaoDeveTerErro()
        {
            var comando = CriarComandoValido();
            comando.PropostaDTO.TiposInscricao = new List<PropostaTipoInscricaoDTO> { new PropostaTipoInscricaoDTO { TipoInscricao = TipoInscricao.Externa } };
            comando.PropostaDTO.LinkParaInscricoesExterna = null;

            var resultado = _validator.TestValidate(comando);
            resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.LinkParaInscricoesExterna);
        }

        [Theory]
        [InlineData("Justificativa")]
        [InlineData("Objetivos")]
        [InlineData("ConteudoProgramatico")]
        [InlineData("ProcedimentoMetadologico")]
        [InlineData("Referencia")]
        public void DadoComandoSemCampoObrigatorioTexto_QuandoValidar_EntaoDeveTerErro(string campo)
        {
            var comando = CriarComandoValido();
            switch (campo)
            {
                case "Justificativa": comando.PropostaDTO.Justificativa = null; break;
                case "Objetivos": comando.PropostaDTO.Objetivos = null; break;
                case "ConteudoProgramatico": comando.PropostaDTO.ConteudoProgramatico = null; break;
                case "ProcedimentoMetadologico": comando.PropostaDTO.ProcedimentoMetadologico = null; break;
                case "Referencia": comando.PropostaDTO.Referencia = null; break;
            }

            var resultado = _validator.TestValidate(comando);
            
            if (campo == "Justificativa") resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.Justificativa);
            if (campo == "Objetivos") resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.Objetivos);
            if (campo == "ConteudoProgramatico") resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.ConteudoProgramatico);
            if (campo == "ProcedimentoMetadologico") resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.ProcedimentoMetadologico);
            if (campo == "Referencia") resultado.ShouldHaveValidationErrorFor(x => x.PropostaDTO.Referencia);
        }
    }
}
