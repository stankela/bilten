-- 02-Apr-2017 (VERZIJA = 4)

alter table propozicije drop column max_tak_tak1;
GO
alter table ocene drop column line_penalty;
GO
alter table ocene drop column time_penalty;
GO
alter table ocene drop column other_penalty;
GO
alter table ocene drop column vreme_vezbe;
GO
alter table druga_ocena drop column line_penalty;
GO
alter table druga_ocena drop column other_penalty;
GO
alter table sud_odbor_na_spravi drop column broj_meraca_vremena;
GO
alter table sud_odbor_na_spravi drop column broj_lin_sudija;
GO
alter table sudija_ucesnik drop column uloga_u_glav_sud_odboru;
GO
update sudija_na_spravi set uloga = uloga - 10 where uloga > 0;
GO
update verzija_baze SET broj_verzije = 4 WHERE verzija_id = 1;
GO
