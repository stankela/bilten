-- 30-Mar-2017 (VERZIJA = 4)

update table ekipe drop column klub_id
update table ekipe drop column drzava_id
GO
update table propozicije drop column max_tak_tak1
GO
UPDATE [verzija_baze] SET broj_verzije = 4 WHERE verzija_id = 1
GO
