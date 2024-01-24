begin transaction;

alter table Term
    add column CurrentAntiStreak int default 0 not null,
    add column TotalAnswers int default 0 not null,
    add column TotalCorrectAnswers int default 0 not null,
    add column CreatedUtc timestamp default '1900-01-01' not null;

commit;