-- 08-Apr-2017 (VERZIJA = 9)

alter table rezultati_ukupno_zbir_vise_kola add column kval_status tinyint;
GO
alter table rezultati_ukupno_zbir_vise_kola add column parter_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column parter_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column parter_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column parter_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column konj_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column konj_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column konj_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column konj_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column karike_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column karike_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column karike_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column karike_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column preskok_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column preskok_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column preskok_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column preskok_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column razboj_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column razboj_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column razboj_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column razboj_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column vratilo_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column vratilo_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column vratilo_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column vratilo_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column dvo_razboj_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column dvo_razboj_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column dvo_razboj_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column dvo_razboj_4 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column greda_1 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column greda_2 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column greda_3 real;
GO
alter table rezultati_ukupno_zbir_vise_kola add column greda_4 real;
GO

create table poredak_ekipno_zbir_vise_kola (
	poredak_id int identity(1,1) primary key,
	deo_takmicenja_kod tinyint
);
GO

create table rezultati_ekipno_zbir_vise_kola (
	rezultat_id int identity(1,1) primary key,
	red_broj smallint,
	rank smallint,
	kval_status tinyint,
    parter_1 real,
    parter_2 real,
    parter_3 real,
    parter_4 real,
    konj_1 real,
    konj_2 real,
    konj_3 real,
    konj_4 real,
    karike_1 real,
    karike_2 real,
    karike_3 real,
    karike_4 real,
    preskok_1 real,
    preskok_2 real,
    preskok_3 real,
    preskok_4 real,
    razboj_1 real,
    razboj_2 real,
    razboj_3 real,
    razboj_4 real,
    vratilo_1 real,
    vratilo_2 real,
    vratilo_3 real,
    vratilo_4 real,
    dvo_razboj_1 real,
    dvo_razboj_2 real,
    dvo_razboj_3 real,
    dvo_razboj_4 real,
    greda_1 real,
    greda_2 real,
    greda_3 real,
    greda_4 real,
	total_1 real,
	total_2 real,
	total_3 real,
	total_4 real,
	total real,
	ekipa_id int references ekipe(ekipa_id),
	poredak_id int references poredak_ekipno_zbir_vise_kola(poredak_id)
);
GO

alter table takmicenje1 add column poredak_ekipno_zbir_vise_kola_id int references poredak_ekipno_zbir_vise_kola(poredak_id);
GO
