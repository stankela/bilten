-- 02-Dec-2016 (VERZIJA = 2)
ALTER TABLE [ekipe] ADD COLUMN penalty real
GO
UPDATE [verzija_baze] SET broj_verzije = broj_verzije + 1 WHERE verzija_id = 1
GO
