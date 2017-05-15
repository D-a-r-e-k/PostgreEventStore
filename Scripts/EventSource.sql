CREATE TABLE public.eventsource
(
  aggregateid character(36) NOT NULL,
  version integer NOT NULL,
  CONSTRAINT pk_eventsource PRIMARY KEY (aggregateid)
)