﻿using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Pipelines
{
    public class ValidacoesPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validadores;

        public ValidacoesPipeline(IEnumerable<IValidator<TRequest>> validadores)
        {
            this.validadores = validadores ?? throw new ArgumentNullException(nameof(validadores));
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validadores.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var erros = validadores
                    .Select(v => v.Validate(context))
                    .SelectMany(result => result.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (erros != null && erros.Any())
                {
                    throw new NegocioException(erros?.Select(c => c.ErrorMessage)?.ToList());
                }
            }

            return next();
        }
    }
}
