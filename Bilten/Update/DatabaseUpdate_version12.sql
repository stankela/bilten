-- 09-Apr-2017 (VERZIJA = 12)

create table poredak_sprava_finale_kupa (
	poredak_id int identity(1,1) primary key,
	sprava tinyint,
	takmicenje1_id int references takmicenje1(takmicenje1_id)
);
GO

create table rezultati_sprava_finale_kupa (
	rezultat_id int identity(1,1) primary key,
	red_broj smallint,
	rank smallint,
	kval_status tinyint,
	d_prvo_kolo real,
	e_prvo_kolo real,
	total_prvo_kolo real,
	d_drugo_kolo real,
	e_drugo_kolo real,
	total_drugo_kolo real,
	total real,
	gimnasticar_id int references gimnasticari_ucesnici(gimnasticar_id),
	poredak_id int references poredak_sprava_finale_kupa(poredak_id)
);
GO
