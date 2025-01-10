alter table AspNetUser add column JoinDate timestamp;
update AspNetUser set JoinDate = '0001-01-01';
alter table AspNetUser alter column JoinDate set not null;