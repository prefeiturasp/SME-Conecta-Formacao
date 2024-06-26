﻿namespace SME.ConectaFormacao.Infra.Servicos.Eol.Constante
{
    public class EndpointsEolConstantes
    {
        public const string OBTER_NOME_PROFISSIONAL = "funcionarios/nome-servidor/{0}";
        public const string OBTER_NOME_ABREVIACAO_DRE = "abrangencia/nome-abreviacao-dres";
        public const string OBTER_COMPONENTE_CURRICULAR_E_ANO_TURMA_POR_ANO_LETIVO = "v1/componentes-curriculares/ano-turma/ano-letivo/{0}";
        public const string OBTER_UNIDADE_POR_CODIGO = "escolas/unidade-eol/{0}";
        public const string OBTER_CARGOS_FUNCIONARIO_POR_RF = "funcionarios/cargo/{0}";
        public const string URL_FUNCIONARIOS_REGISTROS_FUNCIONAIS_CONECTA_FORMACAO = "funcionarios/registros-funcionais/conecta-formacao";
        public const string URL_FUNCIONARIO_EXTERNO_POR_CPF = "funcionarios/funcionario-externo/{0}";
        public const string OBTER_DRE_UE_ATRIBUICAO_POR_FUNCIONARIO_CARGO = "funcionarios/atribuicao/{0}/cargo/{1}";
        public const string OBTER_NOME_SERVIDOR_EOL = "funcionarios/nome-usuario-eol/{0}";
        public const string OBTER_USUARIOS_POR_PERFIS = "funcionarios/usuarios/conecta-formacao";
        public const string VERIFICAR_SE_FUNCIONARIOS_ESTAO_ATIVOS = "acessos/buscar-rfs-cargo-ativo";
    }
}
