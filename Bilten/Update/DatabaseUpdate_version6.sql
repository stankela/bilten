-- 04-Apr-2017 (VERZIJA = 6)

alter table gimnasticari_ucesnici drop column takmicenje_id;
GO
UPDATE [verzija_baze] SET broj_verzije = 6 WHERE verzija_id = 1;
GO
