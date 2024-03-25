using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class MontarEmailEducacionalCommandHandler : IRequestHandler<MontarEmailEducacionalCommand , string>
    {
        public async Task<string> Handle(MontarEmailEducacionalCommand request, CancellationToken cancellationToken)
        {
            var partesNome = request.NomeCompleto.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var primeiroNome = partesNome[0];
            var ultimoNome = partesNome[^1];
            var emailEdu = !string.IsNullOrEmpty(ultimoNome) ? $"{primeiroNome}.{ultimoNome}@edu.sme.prefeitura.sp.gov.br" 
                                                                  : $"{primeiroNome}@edu.sme.prefeitura.sp.gov.br";
            return emailEdu.ToLower();
        }
    }
}