CREATE OR REPLACE FUNCTION public.saveevents(
    aggregatedidparam character,
    expectedversion integer,
    events eventtype[])
  RETURNS void AS
$BODY$
DECLARE 
    CurrentVersion int;

BEGIN
	SELECT Version INTO CurrentVersion FROM EventSource WHERE AggregateId = AggregatedIdParam;
	
	IF CurrentVersion IS NULL THEN
		CurrentVersion := 0;
		INSERT INTO EventSource(AggregateId, Version) VALUES (AggregatedIdParam, CurrentVersion);
	END IF;

	-- concurrency validation
	IF (ExpectedVersion - 1) != CurrentVersion THEN
		RAISE EXCEPTION 'Concurrency problem';
	END IF;

	FOR I IN 1 .. ARRAY_UPPER(EVENTS, 1)
 	LOOP
 		IF AGGREGATEDIDPARAM != EVENTS[I].AGGREGATEID THEN
 			RAISE EXCEPTION 'EVENTS FOR ONLY ONE AGGREGATE ARE PROCESSED AT THE TIME';
 		END IF;
 		
 		CURRENTVERSION := CURRENTVERSION + 1;
 		INSERT INTO EVENT(AGGREGATEID, DATA, VERSION, DATE) VALUES (EVENTS[I].AGGREGATEID, EVENTS[I].DATA, CURRENTVERSION, EVENTS[I].DATE);
 	END LOOP;

	UPDATE EventSource
	SET Version = CurrentVersion
	WHERE AggregateId = AggregatedIdParam;
END;