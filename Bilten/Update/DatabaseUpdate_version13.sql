-- 10-Apr-2017 (VERZIJA = 13)

drop table rezultati_sprava_finale_kupa_update;
GO
alter table takmicenja add column last_modified datetime;
GO