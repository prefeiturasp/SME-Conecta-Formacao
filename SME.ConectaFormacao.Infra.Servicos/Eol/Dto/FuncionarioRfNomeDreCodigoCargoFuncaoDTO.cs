using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto
{
    public class FuncionarioRfNomeDreCodigoCargoFuncaoDTO
    {
        public string Rf { get; set; }
        public string Nome { get; set; }
        
        public string CargoCodigo { get; set; }
        public string CargoDreCodigo { get; set; }
        public string CargoUeCodigo { get; set; }
        
        public string CargoSobrepostoCodigo { get; set; }
        public string CargoSobrepostoDreCodigo { get; set; }
        public string CargoSobrepostoUeCodigo { get; set; }
        
        public string FuncaoCodigo { get; set; }
        public string FuncaoDreCodigo { get; set; }
        public string FuncaoUeCodigo { get; set; }

        public string DreCodigo { get; set; }
    }
}
