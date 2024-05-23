﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Email
{
    public class CasoDeUsoEnviarEmail : CasoDeUsoAbstrato, ICasoDeUsoEnviarEmail
    {
        public CasoDeUsoEnviarEmail(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var enviarEmail = param.ObterObjetoMensagem<EnviarEmailDto>() ?? throw new NegocioException(MensagemNegocio.DADOS_ENVIO_EMAIL_NAO_LOCALIZADO);

            await mediator.Send(new EnviarEmailCommand(enviarEmail.NomeDestinatario, enviarEmail.EmailDestinatario, enviarEmail.Titulo, enviarEmail.Texto));

            return true;
        }
    }
}