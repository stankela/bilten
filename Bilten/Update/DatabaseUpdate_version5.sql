-- 02-Apr-2017 (VERZIJA = 5)

alter table ekipe drop column rez_takmicenje_id;
GO
alter table ekipe drop column klub_id;
GO
alter table ekipe drop column drzava_id;
GO
UPDATE [verzija_baze] SET broj_verzije = 5 WHERE verzija_id = 1;
GO
