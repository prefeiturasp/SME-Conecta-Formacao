using FluentValidation.TestHelper;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricao
{
    public class TransferirInscricaoCommandTeste
    {
        private readonly TransferirInscricaoCommandValidator _validator;

        public TransferirInscricaoCommandTeste()
        {
            _validator = new TransferirInscricaoCommandValidator();
        }

        [Fact(DisplayName = "Erro quando InscricaoTransferenciaDTO for nulo")]
        public void Deve_Retornar_Erro_Se_InscricaoTransferenciaDTO_For_Nulo()
        {
            var command = new TransferirInscricaoCommand(null);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldHaveValidationErrorFor(x => x.InscricaoTransferenciaDTO);
        }

        [Fact(DisplayName = "Erro quando IdTurmaDestino for 0")]
        public void Deve_Retornar_Erro_Se_IdTurmaDestino_For_Zero()
        {
            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaDestino = 0,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
                {
                    new InscricaoTransferenciaDTOCursista { IdInscricao = 1, Rf = "RF1" }
                }
            };

            var command = new TransferirInscricaoCommand(dto);
            var resultado = _validator.TestValidate(command);

            resultado.ShouldHaveValidationErrorFor("InscricaoTransferenciaDTO.IdTurmaDestino");
        }

        [Fact(DisplayName = "Erro quando Cursistas estiver vazio")]
        public void Deve_Retornar_Erro_Se_Cursistas_Estiverem_Vazios()
        {
            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaDestino = 1,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>()
            };

            var command = new TransferirInscricaoCommand(dto);
            var resultado = _validator.TestValidate(command);

            resultado.ShouldHaveValidationErrorFor("InscricaoTransferenciaDTO.Cursistas");
        }

        [Fact(DisplayName = "Não deve validar regras internas se DTO for nulo")]
        public void Nao_Deve_Validar_IdTurmaDestino_Se_DTO_For_Nulo()
        {
            var command = new TransferirInscricaoCommand(null);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldNotHaveValidationErrorFor("InscricaoTransferenciaDTO.IdTurmaDestino");
        }

        [Fact(DisplayName = "Não deve validar cursistas se DTO for nulo")]
        public void Nao_Deve_Validar_Cursistas_Se_DTO_For_Nulo()
        {
            var command = new TransferirInscricaoCommand(null);

            var resultado = _validator.TestValidate(command);

            resultado.ShouldNotHaveValidationErrorFor("InscricaoTransferenciaDTO.Cursistas");
        }

        [Fact(DisplayName = "Deve passar com dados válidos")]
        public void Deve_Passar_Se_Dados_Forem_Validos()
        {
            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaDestino = 10,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
                {
                    new InscricaoTransferenciaDTOCursista { IdInscricao = 1, Rf = "RF1" },
                    new InscricaoTransferenciaDTOCursista { IdInscricao = 2, Rf = "RF2" }
                }
            };

            var command = new TransferirInscricaoCommand(dto);
            var resultado = _validator.TestValidate(command);

            resultado.ShouldNotHaveAnyValidationErrors();
        }
    }
}

