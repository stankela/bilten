-- 08-Apr-2017 (VERZIJA = 11)

create table poredak_ekipno_finale_kupa (
	poredak_id int identity(1,1) primary key,
	deo_takmicenja_kod tinyint
);
GO

create table rezultati_ekipno_finale_kupa (
	rezultat_id int identity(1,1) primary key,
	red_broj smallint,
	rank smallint,
	kval_status tinyint,
    parter_1 real,
    parter_2 real,
    konj_1 real,
    konj_2 real,
    karike_1 real,
    karike_2 real,
    preskok_1 real,
    preskok_2 real,
    razboj_1 real,
    razboj_2 real,
    vratilo_1 real,
    vratilo_2 real,
    dvo_razboj_1 real,
    dvo_razboj_2 real,
    greda_1 real,
    greda_2 real,
	total_1 real,
	total_2 real,
	total real,
	ekipa_id int references ekipe(ekipa_id),
	poredak_id int references poredak_ekipno_finale_kupa(poredak_id)
);
GO

alter table takmicenje1 add column poredak_ekipno_finale_kupa_id int references poredak_ekipno_finale_kupa(poredak_id);
GO
