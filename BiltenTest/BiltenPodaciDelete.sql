ALTER TABLE [takmicenja] DROP CONSTRAINT [FK__takmicenja__0000000000000C38]
GO
ALTER TABLE [takmicenja] DROP CONSTRAINT [FK__takmicenja__0000000000000C3D]
GO
ALTER TABLE [takmicenje1] DROP CONSTRAINT [FK__takmicenje1__0000000000000A8C]
GO
ALTER TABLE [takmicenje3] DROP CONSTRAINT [FK__takmicenje3__0000000000000A86]
GO
DELETE FROM [ocene]
GO
DELETE FROM [druga_ocena]
GO
DELETE FROM [gimnasticari]
GO
DELETE FROM [sudije]
GO
DELETE FROM [drzave]
GO
DELETE FROM [ekipa_gimnasticar]
GO
DELETE FROM [nastup_na_spravi]
GO
DELETE FROM [rezultati_preskok]
GO
DELETE FROM [rezultati_sprava]
GO
DELETE FROM [rezultati_sprava_finale_kupa_update]
GO
DELETE FROM [rezultati_ukupno]
GO
DELETE FROM [ucesnici_tak1]
GO
DELETE FROM [ucesnici_tak2]
GO
DELETE FROM [ucesnici_tak3]
GO
DELETE FROM [gimnasticari_ucesnici]
GO
DELETE FROM [rezultati_ekipno]
GO
DELETE FROM [ucesnici_tak4]
GO
DELETE FROM [ekipe]
GO
DELETE FROM [sudija_na_spravi]
GO
DELETE FROM [sudija_ucesnik]
GO
DELETE FROM [drzave_ucesnici]
GO
DELETE FROM [kategorije_gimnasticara]
GO
DELETE FROM [klubovi]
GO
DELETE FROM [klubovi_ucesnici]
GO
DELETE FROM [mesta]
GO
DELETE FROM [opcije]
GO
DELETE FROM [poredak_sprava]
GO
DELETE FROM [rezultatsko_takmicenje]
GO
DELETE FROM [takmicenje1]
GO
DELETE FROM [takmicenje4]
GO
DELETE FROM [poredak_ekipno]
GO
DELETE FROM [takmicenje2]
GO
DELETE FROM [poredak_ukupno]
GO
DELETE FROM [rezultatsko_takmicenje_description]
GO
DELETE FROM [propozicije]
GO
DELETE FROM [start_lista_sprava]
GO
DELETE FROM [raspored_nastupa]
GO
DELETE FROM [sud_odbor_na_spravi]
GO
DELETE FROM [raspored_sudija]
GO
DELETE FROM [takmicarske_kategorije]
GO
DELETE FROM [takmicenja]
GO
DELETE FROM [takmicenje3]
GO
ALTER TABLE [takmicenja] ADD CONSTRAINT [FK__takmicenja__0000000000000C38] FOREIGN KEY ([prvo_kolo_id]) REFERENCES [takmicenja]([takmicenje_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [takmicenja] ADD CONSTRAINT [FK__takmicenja__0000000000000C3D] FOREIGN KEY ([drugo_kolo_id]) REFERENCES [takmicenja]([takmicenje_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [takmicenje1] ADD CONSTRAINT [FK__takmicenje1__0000000000000A8C] FOREIGN KEY ([poredak_preskok_id]) REFERENCES [poredak_sprava]([poredak_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [takmicenje3] ADD CONSTRAINT [FK__takmicenje3__0000000000000A86] FOREIGN KEY ([poredak_preskok_id]) REFERENCES [poredak_sprava]([poredak_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
