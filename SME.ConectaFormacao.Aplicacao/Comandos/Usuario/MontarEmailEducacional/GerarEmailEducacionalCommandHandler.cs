using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
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
            var ultimoNome = partesNome.Length > 1 ? partesNome.LastOrDefault() : String.Empty;
            var emailEdu = CriarEmailPorTipo(request.Usuario,cpfOuRf, primeiroNome, ultimoNome);
            
            return emailEdu.RemoverAcentosECaracteresEspeciais().ToLower();
        }

        private string CriarEmailPorTipo(Usuario usuario, string cpfOuRf, string? primeiroNome, string? ultimoNome)
        {
            if (usuario.TipoEmail == TipoEmail.Estagiario)
            {
                return ultimoNome.EstaPreenchido()
                    ? $"{primeiroNome}{ultimoNome}.e{cpfOuRf}{DOMINIO_EMAIL}"
                    : $"{primeiroNome}.e{cpfOuRf}{DOMINIO_EMAIL}";
            }
            
            return ultimoNome.EstaPreenchido()
                ? $"{primeiroNome}{ultimoNome}.{cpfOuRf}{DOMINIO_EMAIL}"
                : $"{primeiroNome}.{cpfOuRf}{DOMINIO_EMAIL}";
        }
    }
}