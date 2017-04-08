-- 08-Apr-2017 (VERZIJA = 8)

create table poredak_ukupno_zbir_vise_kola (
	poredak_id int identity(1,1) primary key,
    deo_takmicenja_kod tinyint
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
alter table takmicenje1 add column poredak_ukupno_zbir_vise_kola_id int references poredak_ukupno_zbir_vise_kola(poredak_id);
GO
