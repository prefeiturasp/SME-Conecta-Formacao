using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarEmailEducacionalCommandHandler : IRequestHandler<GerarEmailEducacionalCommand , string>
    {
        private const string DOMINIO_EMAIL = "@edu.sme.prefeitura.sp.gov.br";
        public async Task<string> Handle(GerarEmailEducacionalCommand request, CancellationToken cancellationToken)
        {
            var cpfOuRf = request.Usuario.Tipo == TipoUsuario.Externo ? request.Usuario.Cpf : request.Usuario.Login;
            var partesNome = request.Usuario.Nome.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var primeiroNome = partesNome.FirstOrDefault();
            var ultimoNome = partesNome.LastOrDefault();
            var emailEdu = !string.IsNullOrEmpty(ultimoNome) ? $"{primeiroNome}.{ultimoNome}.{cpfOuRf}.{DOMINIO_EMAIL}" 
                                                                  : $"{primeiroNome}.{cpfOuRf}.{DOMINIO_EMAIL}";
            return emailEdu.RemoverAcentosECaracteresEspeciais().ToLower();
        }
    }
}