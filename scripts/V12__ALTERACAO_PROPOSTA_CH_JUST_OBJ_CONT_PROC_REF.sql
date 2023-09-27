alter table proposta add if not exists carga_horaria_presencial varchar(6) null;
alter table proposta add if not exists carga_horaria_sincrona varchar(6) null;
alter table proposta add if not exists carga_horaria_distancia varchar(6) null;
alter table proposta add if not exists justificativa text null;
alter table proposta add if not exists objetivos text null;
alter table proposta add if not exists conteudo_programatico text null;
alter table proposta add if not exists procedimento_metodologico text null;
alter table proposta add if not exists referencia text null;

create table if not exists public.palavra_chave (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	nome varchar(100) not null,	
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_login varchar(200) NOT NULL,
	alterado_login varchar(200) NULL,
	excluido boolean NOT null default false,
	constraint palavra_chave_pk primary key (id)	
);

create table if not exists public.proposta_palavra_chave (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY(NO MINVALUE NO MAXVALUE NO CYCLE),
	proposta_id int8 not null,
	palavra_chave_id int8 not null,	
	constraint proposta_palavra_chave_pk primary key (id),
	constraint proposta_palavra_chave_proposta_fk foreign key (proposta_id) references proposta (id),
	constraint proposta_palavra_chave_palavra_chave_fk foreign key (palavra_chave_id) references palavra_chave (id)	
);

CREATE index if not exists proposta_palavra_chave_proposta_idx ON public.proposta (id);
CREATE index if not exists proposta_palavra_chave_palavra_chave_idx ON public.palavra_chave (id);

insert into public.palavra_chave (nome,criado_em, criado_por, criado_login) 
select 'ACOMPANHAMENTO DAS APRENDIZAGENS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ACOMPANHAMENTO DAS APRENDIZAGENS') union all
select 'AFRICANIDADES',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'AFRICANIDADES') union all
select 'ALFABETIZAÇÃO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ALFABETIZAÇÃO') union all
select 'ALTAS HABILIDADES',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ALTAS HABILIDADES') union all
select 'ARTE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ARTE') union all
select 'AVALIAÇÃO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'AVALIAÇÃO') union all
select 'AVALIAÇÃO EDUCACIONAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'AVALIAÇÃO EDUCACIONAL') union all
select 'AVALIAÇÃO EXTERNA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'AVALIAÇÃO EXTERNA') union all
select 'BEBÊS E CRIANÇAS DE 0 A 3 ANOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'BEBÊS E CRIANÇAS DE 0 A 3 ANOS') union all
select 'BULLYING',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'BULLYING') union all
select 'BUSCA ATIVA ESCOLAR',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'BUSCA ATIVA ESCOLAR') union all
select 'CIÊNCIAS HUMANAS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CIÊNCIAS HUMANAS') union all
select 'CIÊNCIAS NATURAIS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CIÊNCIAS NATURAIS') union all
select 'CINEMA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CINEMA') union all
select 'CONTAÇÃO DE HISTÓRIAS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CONTAÇÃO DE HISTÓRIAS') union all
select 'COORDENAÇÃO PEDAGÓGICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'COORDENAÇÃO PEDAGÓGICA') union all
select 'CORPO E MOVIMENTO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CORPO E MOVIMENTO') union all
select 'CULTURA DE PAZ',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CULTURA DE PAZ') union all
select 'CULTURA MAKER',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CULTURA MAKER') union all
select 'CURRÍCULO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'CURRÍCULO') union all
select 'DANÇA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DANÇA') union all
select 'DEFICIÊNCIA AUDITIVA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DEFICIÊNCIA AUDITIVA') union all
select 'DEFICIÊNCIA FÍSICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DEFICIÊNCIA FÍSICA') union all
select 'DEFICIÊNCIA INTELECTUAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DEFICIÊNCIA INTELECTUAL') union all
select 'DEFICIÊNCIA MÚLTIPLA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DEFICIÊNCIA MÚLTIPLA') union all
select 'DEFICIÊNCIA VISUAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DEFICIÊNCIA VISUAL') union all
select 'DIREÇÃO DE UNIDADE ESCOLAR',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DIREÇÃO DE UNIDADE ESCOLAR') union all
select 'DIREITOS DA CRIANÇA E DO ADOLESCENTE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DIREITOS DA CRIANÇA E DO ADOLESCENTE') union all
select 'DIREITOS HUMANOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DIREITOS HUMANOS') union all
select 'DIVERSIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DIVERSIDADE') union all
select 'DOCUMENTAÇÃO PEDAGÓGICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'DOCUMENTAÇÃO PEDAGÓGICA') union all
select 'EDUCAÇÃO ALIMENTAR',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO ALIMENTAR') union all
select 'EDUCAÇÃO AMBIENTAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO AMBIENTAL') union all
select 'EDUCAÇÃO DE JOVENS E ADULTOS - EJA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO DE JOVENS E ADULTOS - EJA') union all
select 'EDUCAÇÃO ESPECIAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO ESPECIAL') union all
select 'EDUCAÇÃO FÍSICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO FÍSICA') union all
select 'EDUCAÇÃO INCLUSIVA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO INCLUSIVA') union all
select 'EDUCAÇÃO INDÍGENA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO INDÍGENA') union all
select 'EDUCAÇÃO INFANTIL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO INFANTIL') union all
select 'EDUCAÇÃO INTEGRAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO INTEGRAL') union all
select 'EDUCAÇÃO PARA AS RELAÇÕES ÉTNICO-RACIAIS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO PARA AS RELAÇÕES ÉTNICO-RACIAIS') union all
select 'EDUCAÇÃO PARA O TRÂNSITO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCAÇÃO PARA O TRÂNSITO') union all
select 'EDUCOMUNICAÇÃO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EDUCOMUNICAÇÃO') union all
select 'ENSINO FUNDAMENTAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ENSINO FUNDAMENTAL') union all
select 'ENSINO HÍBRIDO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ENSINO HÍBRIDO') union all
select 'ENSINO MÉDIO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ENSINO MÉDIO') union all
select 'EQUIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'EQUIDADE') union all
select 'ESPORTE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ESPORTE') union all
select 'ESTUDO DO MEIO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ESTUDO DO MEIO') union all
select 'FILOSOFIA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'FILOSOFIA') union all
select 'GEOGRAFIA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GEOGRAFIA') union all
select 'GESTÃO DE CONFLITOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GESTÃO DE CONFLITOS') union all
select 'GESTÃO DE PROCESSOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GESTÃO DE PROCESSOS') union all
select 'GESTÃO DE SALA DE AULA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GESTÃO DE SALA DE AULA') union all
select 'GESTÃO DEMOCRÁTICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GESTÃO DEMOCRÁTICA') union all
select 'GESTÃO ESCOLAR',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GESTÃO ESCOLAR') union all
select 'GESTÃO PEDAGÓGICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GESTÃO PEDAGÓGICA') union all
select 'GRÊMIO ESTUDANTIL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'GRÊMIO ESTUDANTIL') union all
select 'HISTÓRIA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'HISTÓRIA') union all
select 'HORTA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'HORTA') union all
select 'IDENTIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'IDENTIDADE') union all
select 'IMIGRANTES',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'IMIGRANTES') union all
select 'IMPRENSA JOVEM',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'IMPRENSA JOVEM') union all
select 'INTERCULTURALIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'INTERCULTURALIDADE') union all
select 'INTERDISCIPLINARIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'INTERDISCIPLINARIDADE') union all
select 'JOGOS DE TABULEIRO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'JOGOS DE TABULEIRO') union all
select 'JOGOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'JOGOS') union all
select 'LEITURA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'LEITURA') union all
select 'LÍNGUA BRASILEIRA DE SINAIS - LIBRAS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'LÍNGUA BRASILEIRA DE SINAIS - LIBRAS') union all
select 'LÍNGUA INGLESA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'LÍNGUA INGLESA') union all
select 'LÍNGUA PORTUGUESA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'LÍNGUA PORTUGUESA') union all
select 'LINGUAGEM DIGITAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'LINGUAGEM DIGITAL') union all
select 'LUDICIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'LUDICIDADE') union all
select 'MATEMÁTICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'MATEMÁTICA') union all
select 'MATRIZ DE SABERES',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'MATRIZ DE SABERES') union all
select 'MEMÓRIA INSTITUCIONAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'MEMÓRIA INSTITUCIONAL') union all
select 'METODOLOGIAS ATIVAS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'METODOLOGIAS ATIVAS') union all
select 'MIGRANTES',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'MIGRANTES') union all
select 'MÚSICA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'MÚSICA') union all
select 'OBJETIVOS DE DESENVOLVIMENTO SUSTENTÁVEL - ODS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'OBJETIVOS DE DESENVOLVIMENTO SUSTENTÁVEL - ODS') union all
select 'ÓRGÃOS COLEGIADOS DA ESCOLA (APM/CONSELHO)',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ÓRGÃOS COLEGIADOS DA ESCOLA (APM/CONSELHO)') union all
select 'PRODUÇÃO TEXTUAL',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'PRODUÇÃO TEXTUAL') union all
select 'PROJETO POLÍTICO PEDAGÓGICO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'PROJETO POLÍTICO PEDAGÓGICO') union all
select 'PROTAGONISMO DO ESTUDANTE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'PROTAGONISMO DO ESTUDANTE') union all
select 'RECUPERAÇÃO DAS APRENDIZAGENS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'RECUPERAÇÃO DAS APRENDIZAGENS') union all
select 'REDE DE PROTEÇÃO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'REDE DE PROTEÇÃO') union all
select 'REFUGIADOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'REFUGIADOS') union all
select 'RELAÇÕES DE GÊNERO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'RELAÇÕES DE GÊNERO') union all
select 'RESPONSABILIDADES FUNCIONAIS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'RESPONSABILIDADES FUNCIONAIS') union all
select 'ROTINAS/PROCESSOS ADMINISTRATIVOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'ROTINAS/PROCESSOS ADMINISTRATIVOS') union all
select 'SEXUALIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'SEXUALIDADE') union all
select 'SUPERVISÃO ESCOLAR',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'SUPERVISÃO ESCOLAR') union all
select 'SUSTENTABILIDADE',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'SUSTENTABILIDADE') union all
select 'TEATRO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TEATRO') union all
select 'TECNOLOGIA ASSISTIVA',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TECNOLOGIA ASSISTIVA') union all
select 'TECNOLOGIA DA INFORMAÇÃO E COMUNICAÇÃO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TECNOLOGIA DA INFORMAÇÃO E COMUNICAÇÃO') union all
select 'TEMPOS, ESPAÇOS, MATERIALIDADES E INTERAÇÕES',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TEMPOS, ESPAÇOS, MATERIALIDADES E INTERAÇÕES') union all
select 'TERRITÓRIO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TERRITÓRIO') union all
select 'TRABALHO COM PROJETOS',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TRABALHO COM PROJETOS') union all
select 'TRANSTORNO GLOBAL DO DESENVOLVIMENTO',NOW(), 'SISTEMA', '0' where not exists (select 1 from public.palavra_chave where nome = 'TRANSTORNO GLOBAL DO DESENVOLVIMENTO');
