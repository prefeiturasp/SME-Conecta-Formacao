﻿using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraCommandHandler : IRequestHandler<GerarNotificacaoAreaPromotoraCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoAreaPromotoraCommandHandler(ITransacao transacao,IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,IMediator mediator,IMapper mapper,IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(GerarNotificacaoAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var linkSistema = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacao, DateTimeExtension.HorarioBrasilia().Year));
            
            var notificacao = await ObterNotificacao(request.Proposta, linkSistema.Valor);
            
            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);
                
                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);
                
                transacao.Commit();

                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, notificacao));
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }

            return true;
        }
        
        private async Task<Notificacao> ObterNotificacao(Proposta proposta, string linkSistema)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterAreaPromotoraPorPropostaId(proposta.Id);

            var usuariosAreasPromotoras = await _mediator.Send(new ObterUsuariosPorPerfilQuery(new[] { areaPromotora.GrupoId }));
            
            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                
                Titulo = string.Format("Proposta {0} - {1} foi analisada pela Comissão de Análise", 
                    proposta.Id, 
                    proposta.NomeFormacao),
                
                Mensagem = string.Format("A proposta {0} - {1} foi analisada pela Comissão de Análise. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e verifique os comentários.",
                    proposta.Id, 
                    proposta.NomeFormacao, 
                    linkSistema),
                
                Parametros = JsonConvert.SerializeObject(proposta),
                Usuarios =  _mapper.Map<IEnumerable<NotificacaoUsuario>>(usuariosAreasPromotoras)
            };
        }
    }
}