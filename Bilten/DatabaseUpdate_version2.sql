-- 02-Dec-2016 (VERZIJA = 2)
-- Prebaciti fajl u bin direktorijum

ALTER TABLE [ekipe] ADD COLUMN penalty real
GO
ALTER TABLE [rezultati_ekipno] ADD COLUMN penalty real
GO
UPDATE [verzija_baze] SET broj_verzije = broj_verzije + 1 WHERE verzija_id = 1
GO
