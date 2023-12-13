CREATE EXTENSION IF NOT EXISTS "unaccent";

CREATE OR REPLACE FUNCTION public.f_unaccent(text)
 RETURNS text
 LANGUAGE sql
 IMMUTABLE
AS $function$
SELECT public.unaccent('public.unaccent', $1)  -- schema-qualify function and dictionary
$function$
;