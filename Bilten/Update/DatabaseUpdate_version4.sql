-- 30-Mar-2017 (VERZIJA = 4)

alter table ekipe drop column klub_id
alter table ekipe drop column drzava_id
GO
alter table propozicije drop column max_tak_tak1
GO
alter table ocene drop column line_penalty
alter table ocene drop column time_penalty
alter table ocene drop column other_penalty
alter table ocene drop column vreme_vezbe
GO
alter table druga_ocena drop column line_penalty
alter table druga_ocena drop column other_penalty
GO
alter table sud_odbor_na_spravi drop column broj_meraca_vremena
alter table sud_odbor_na_spravi drop column broj_lin_sudija
GO
update verzija_baze SET broj_verzije = 4 WHERE verzija_id = 1
GO
