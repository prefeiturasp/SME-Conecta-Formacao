module.exports = {
  projectId: "SME-CONECTA", // the projectId, can be any values for sorry-cypress users
  recordKey: "somekey", // the record key, can be any value for sorry-cypress users
  cloudServiceUrl: "http://10.50.1.202:1234",   // Sorry Cypress users - set the director service URL
  
  // CI Build ID
  ciBuildId: process.env.CI_BUILD_ID || `local-${Date.now()}`,
  
  // Configurações adicionais
  parallel: false,
  record: false
};
// Este arquivo é usado para configurar o Sorry Cypress, uma ferramenta de teste de integração para aplicações web. Ele define o projectId, recordKey, cloudServiceUrl e ciBuildId, que são necessários para a execução dos testes. O ciBuildId é gerado dinamicamente com base na variável de ambiente ou usando um timestamp local. As opções parallel e record estão definidas como false, indicando que os testes não serão executados em paralelo e não serão gravados.
// Teste