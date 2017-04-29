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

alter table rezultati_ekipno drop column penalty;
GO
alter table ekipe add column sprave_mask int;
GO
update ekipe set sprave_mask = 510;
GO
alter table propozicije add column preskok_tak2_bolja_ocena bit;
GO
alter table gimnasticari_ucesnici add column penalty_viseboj real;
GO
alter table takmicenja add column last_modified datetime;
GO
