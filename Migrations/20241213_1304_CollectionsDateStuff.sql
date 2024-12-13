alter table Collection add CreatedTimeUtc timestamp;
update Collection set CreatedTimeUtc = '1900-01-01';
alter table Collection alter column CreatedTimeUtc set not null;

alter table Collection add ViewedTimeUtc timestamp;
update Collection set ViewedTimeUtc = '1900-01-01';
alter table Collection alter column ViewedTimeUtc set not null;