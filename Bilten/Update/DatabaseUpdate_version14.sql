-- 21-Apr-2017 (VERZIJA = 14)

alter table rezultati_ekipno drop column penalty;
GO
alter table ekipe add column sprave_mask int;
GO
update ekipe set sprave_mask = 510;
GO

