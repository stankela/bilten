-- 30-Mar-2017 (VERZIJA = 4)

update table ekipe drop column klub_id
update table ekipe drop column drzava_id
GO
update table propozicije drop column max_tak_tak1
GO
update table ocene drop column line_penalty
update table ocene drop column time_penalty
update table ocene drop column other_penalty
update table ocene drop column vreme_vezbe
GO
update table druga_ocena drop column line_penalty
update table druga_ocena drop column other_penalty
GO
UPDATE [verzija_baze] SET broj_verzije = 4 WHERE verzija_id = 1
GO
