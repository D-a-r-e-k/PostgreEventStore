CREATE TABLE public.event
(
  aggregateid character(36) NOT NULL,
  data bytea NOT NULL,
  version integer NOT NULL,
  date date NOT NULL,
  CONSTRAINT pk_event PRIMARY KEY (aggregateid, version),
  CONSTRAINT fk_event_eventsource FOREIGN KEY (aggregateid)
      REFERENCES public.eventsource (aggregateid) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)