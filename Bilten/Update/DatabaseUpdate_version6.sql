-- 04-Apr-2017 (VERZIJA = 6)

alter table gimnasticari_ucesnici drop column takmicenje_id;
GO
alter table gimnasticari_ucesnici drop column reg_broj;
GO
alter table gimnasticari_ucesnici drop column god_reg;
GO
alter table gimnasticari_ucesnici drop column takmicarski_broj;
GO
alter table gimnasticari_ucesnici drop column gimnastika;
GO
UPDATE [verzija_baze] SET broj_verzije = 6 WHERE verzija_id = 1;
GO
