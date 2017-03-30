-- 29-Mar-2017 (VERZIJA = 5)

create table poredak_ukupno_zbir_vise_kola (
	poredak_id int identity(1,1) primary key
);
GO
create table rezultati_ukupno_zbir_vise_kola (
	rezultat_id int identity(1,1) primary key,
	red_broj smallint,
	rank smallint,
	total_1 real,
	total_2 real,
	total_3 real,
	total_4 real,
	total real,
	gimnasticar_id int references gimnasticari_ucesnici(gimnasticar_id),
	poredak_id int references poredak_ukupno_zbir_vise_kola(poredak_id)
);
GO
UPDATE [verzija_baze] SET broj_verzije = 5 WHERE verzija_id = 1
GO
